using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using proyectoAsistencia.Data;
using proyectoAsistencia.Models;
using proyectoAsistencia.Service;
using proyectoAsistencia.Utils;
using proyectoAsistencia.ViewModel;
using X.PagedList;

namespace proyectoAsistencia.Controllers
{
    [Authorize(Policy = "AdministradorPolicy")]
    public class EventoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;
        private readonly EventoPonenteService _eventoPonenteService;

        public EventoController(AppDbContext context, IConfiguration configuration, INotyfService notyfService, EventoPonenteService eventoPonenteService)
        {
            _context = context;
            _configuration = configuration;
            _notyfService = notyfService;
            _eventoPonenteService = eventoPonenteService;
        }

        // GET: Evento
        public async Task<IActionResult> Index(string? cadenaBusqueda, DateTime? fechaBusqueda, Estado? estadoBusqueda, int? page)
        {
            int numeroPagina = page ?? 1;
            int tamañoPagina = _configuration.GetValue("RegistrosPorPagina", 15);

            var consulta = _context.Evento.AsQueryable();

            if (!string.IsNullOrEmpty(cadenaBusqueda))
            {
                ViewData["valorBusquedaCadena"] = cadenaBusqueda;
                consulta = consulta.Where(e => e.Nombre.Contains(cadenaBusqueda));
            }

            if (fechaBusqueda.HasValue)
            {
                ViewData["valorBusquedaFecha"] = fechaBusqueda.Value.ToString("yyyy-MM-dd");
                consulta = consulta.Where(e => e.FechaHoraInicio.Date == fechaBusqueda.Value.Date);
            }

            if (estadoBusqueda.HasValue)
            {
                ViewData["valorBusquedaEstado"] = estadoBusqueda.Value;
                consulta = consulta.Where(e => e.Estado == estadoBusqueda);
            }

            return View(await consulta.OrderByDescending(e => e.Id).ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        
        public IActionResult LimpiarBusqueda()
        {
            return RedirectToAction("Index", new { cadenaBusqueda = (string?)null });
        }

        // GET: Evento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //esplicacion 
            //https://learn.microsoft.com/es-es/ef/core/querying/related-data/eager

            var evento = await _context.Evento
                .Include(e => e.EventoPonentes)
                    .ThenInclude(ep => ep.Ponente)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // GET: Evento/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ListPonente"] = _context.Ponente.ToList();
            return View();
        }

        // POST: Evento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Evento evento, string ponenteId, string opEliminarPonenteID, string ponenteIds)
        {
            ViewData["ListPonente"] = _context.Ponente.ToList();

            if (evento.FechaHoraInicio < DateTime.Now)
            {
                _notyfService.Warning("La fecha y hora de inicio debe ser mayor o igual a la actual.");
                return View(evento);
            }

            if (evento.FechaHoraFin <= DateTime.Now)
            {
                _notyfService.Warning("La fecha y hora de finalización no puede ser una fecha ya pasada.");
                return View(evento);
            }

            if (evento.FechaHoraFin <= evento.FechaHoraInicio)
            {
                _notyfService.Warning("La fecha y hora de finalización debe ser mayor que la de inicio.");
                return View(evento);
            }

            if (!string.IsNullOrWhiteSpace(ponenteId))
            {
                string[] partes = ponenteId.Split(',');
                int idPonente = int.Parse(partes[0]);

                var ponenteExistente = _context.Ponente.Find(idPonente);
                if (ponenteExistente != null)
                {
                    EventoPonente nuevoPonente = new EventoPonente
                    {
                        PonenteId = idPonente,
                        Ponente = ponenteExistente,
                        EventoId = 0 // Actualizará después
                    };

                    _eventoPonenteService.Agregar(nuevoPonente);
                }
                ActualizarListas();
            }
            else if (!string.IsNullOrWhiteSpace(opEliminarPonenteID))
            {
                int index = int.Parse(opEliminarPonenteID);
                _eventoPonenteService.Eliminar(index);
                ActualizarListas();
            }
            else if (!ModelState.IsValid)
            {
                var administradorId = User.FindFirst("AdministradorId")?.Value;
                evento.AdministradorId = int.Parse(administradorId);

                _context.Evento.Add(evento);
                await _context.SaveChangesAsync();

                int eventoId = evento.Id;

                var idsPonentes = ponenteIds?.Split(',').Select(int.Parse).ToList() ?? new List<int>();

                foreach (var id in idsPonentes)
                {
                    var ponenteExistente = _context.Ponente.Find(id);
                    if (ponenteExistente != null)
                    {
                        EventoPonente eventoPonente = new EventoPonente
                        {
                            EventoId = eventoId,
                            PonenteId = id,
                            Ponente = ponenteExistente // Aquí solo para referencia
                        };

                        _context.EventoPonente.Add(eventoPonente);
                    }
                }

                await _context.SaveChangesAsync();
                _eventoPonenteService.EliminarTodos();
                _notyfService.Success("El evento se creó con éxito.");
                return RedirectToAction(nameof(Index));
            }

            ActualizarListas();
            return View(evento);
        }


        private void ActualizarListas()
        {
            // Obtener la lista de ponentes seleccionados del servicio en memoria
            var listaPonentesSeleccionados = _eventoPonenteService.ListaEventoPonente.ToList();
            ViewBag.ListaPonentesSeleccionados = listaPonentesSeleccionados;

            // Obtener todos los ponentes disponibles que no están seleccionados
            var listaPonentes = _context.Ponente.ToList();
            var ponentesDisponibles = listaPonentes.Where(p => !listaPonentesSeleccionados.Any(ps => ps.PonenteId == p.Id)).ToList();

            // Asignar a ViewBag para usarlos en la vista
            ViewBag.ListPonente = ponentesDisponibles;
        }


        // GET: Evento/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = _context.Evento
                            .Include(e => e.EventoPonentes)
                                .ThenInclude(ep => ep.Ponente)
                            .FirstOrDefault(e => e.Id == id);

            if (evento == null)
            {
                return NotFound();
            }

            var model = new CreateEventoViewModel
            {
                Evento = evento,
                Ponentes = _context.Ponente.ToList(),

                SelectedPonentesIds = evento.EventoPonentes.Select(ep => ep.Ponente.Id).ToList()
            };

            return View(model);
        }


        // POST: Evento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEventoViewModel model)
        {
            if (id != model.Evento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var eventoOriginal = await _context.Evento
                                        .Include(e => e.EventoPonentes)
                                        .FirstOrDefaultAsync(e => e.Id == id);

                    //EL ID ESTÄ  VARCODEADO Y CUANDO CREEMOS LAS SESSIONES ENTONCES DESDDE AHI UTILIACERMOS LOS ID
                    var administradorId = User.FindFirst("AdministradorId")?.Value;
                    model.Evento.AdministradorId = int.Parse(administradorId);

                    if (eventoOriginal == null)
                    {
                        _notyfService.Success("Error, hubo algún problema");
                        return NotFound();
                    }

                    _context.Entry(eventoOriginal).CurrentValues.SetValues(model.Evento);

                    eventoOriginal.EventoPonentes.Clear();

                    if (model.SelectedPonentesIds != null)
                    {
                        foreach (var ponenteId in model.SelectedPonentesIds)
                        {
                            eventoOriginal.EventoPonentes.Add(new EventoPonente { PonenteId = ponenteId });
                        }
                    }

                    await _context.SaveChangesAsync();
                    _notyfService.Success("Se editó con éxito");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Evento.Any(e => e.Id == model.Evento.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            model.Ponentes = _context.Ponente.ToList();
            return View(model);
        }


        // GET: Evento/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evento = await _context.Evento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (evento == null)
            {
                return NotFound();
            }

            return View(evento);
        }

        // POST: Evento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evento = await _context.Evento.FindAsync(id);
            if (evento != null)
            {
                _context.Evento.Remove(evento);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Se eliminó con éxito");
            return RedirectToAction(nameof(Index));
        }

        private bool EventoExists(int id)
        {
            return _context.Evento.Any(e => e.Id == id);
        }
    }
}

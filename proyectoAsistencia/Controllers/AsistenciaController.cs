using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using proyectoAsistencia.Data;
using proyectoAsistencia.Models;
using proyectoAsistencia.Utils;
using X.PagedList;
using System.Linq;

namespace proyectoAsistencia.Controllers
{

    [Authorize(Policy = "AdministradorPolicy")]
    public class AsistenciaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notify;
        private readonly IConfiguration _configuration;

        public AsistenciaController(AppDbContext context, INotyfService notyfy, IConfiguration configuration)
        {
            _context = context;
            _notify = notyfy;
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public string? SearchEmailOrCode { get; set; }

        public async Task<IActionResult> Index(int? page, DateTime? fechaBusqueda, Ciclo? cicloBusqueda, string? busquedaString, int? eventoBusqueda)
        {
            // Cargar eventos con estado ACTIVO y CULMINADO
            var eventos = await _context.Evento
                .Where(e => e.Estado == Estado.ACTIVO || e.Estado == Estado.CULMINADO)
                .ToListAsync();

            ViewData["Eventos"] = new SelectList(eventos, "Id", "Nombre");

            // Consultar asistencias
            var query = _context.Asistencia
                .Include(a => a.Administrador)
                .Include(a => a.Estudiante)
                .Include(a => a.Evento) as IQueryable<Asistencia>;

            // Filtro por nombre, apellido o código de estudiante
            if (!string.IsNullOrEmpty(busquedaString))
            {
                ViewData["searchStringValue"] = busquedaString;
                query = query.Where(a => a.Estudiante.CodigoEstudiante.Contains(busquedaString)
                                         || a.Estudiante.Apellido.Contains(busquedaString)
                                         || a.Estudiante.Nombre.Contains(busquedaString));
            }

            // Filtro por fecha
            if (fechaBusqueda.HasValue)
            {
                ViewData["fechaBusquedaValue"] = fechaBusqueda.Value.ToString("yyyy-MM-dd");
                query = query.Where(a => a.Evento.FechaHoraInicio.Date == fechaBusqueda.Value.Date);
            }

            // Filtro por ciclo
            if (cicloBusqueda.HasValue)
            {
                ViewData["cicloBusquedaValue"] = cicloBusqueda.Value;
                query = query.Where(a => a.Estudiante.Ciclo == cicloBusqueda.Value);
            }

            // Filtro por evento
            if (eventoBusqueda.HasValue && eventoBusqueda.Value > 0)
            {
                ViewData["eventoBusquedaValue"] = eventoBusqueda.Value;
                query = query.Where(a => a.EventoId == eventoBusqueda.Value);
            }

            int numeroPagina = page ?? 1;
            int tamañoPagina = _configuration.GetValue("RegistrosPorPagina", 15);

            return View(await query.OrderByDescending(e => e.Id).ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        public IActionResult LimpiarBusqueda()
        {
            return RedirectToAction("Index", new { busquedaString = (string?)null });
        }


        //crear la exportacion
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ExportarExcel(DateTime? fechaBusqueda, int? eventoBusqueda, Ciclo? cicloBusqueda, string? busquedaString)
        {
            var query = _context.Asistencia
                .Include(a => a.Estudiante)
                .Include(a => a.Evento)
                .AsQueryable();

            if (!string.IsNullOrEmpty(busquedaString))
            {
                query = query.Where(a => a.Estudiante.CodigoEstudiante.Contains(busquedaString) ||
                                         a.Estudiante.Apellido.Contains(busquedaString) ||
                                         a.Estudiante.Nombre.Contains(busquedaString));
            }
            if (fechaBusqueda.HasValue)
            {
                query = query.Where(a => a.Evento.FechaHoraInicio.Date == fechaBusqueda.Value.Date);
            }
            if (eventoBusqueda.HasValue)
            {
                query = query.Where(a => a.EventoId == eventoBusqueda.Value);
            }
            if (cicloBusqueda.HasValue)
            {
                query = query.Where(a => a.Estudiante.Ciclo == cicloBusqueda.Value);
            }

            var asistencias = await query.ToListAsync();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Asistencias");

                worksheet.Cells[1, 1].Value = "Nombre";
                worksheet.Cells[1, 2].Value = "Apellido";
                worksheet.Cells[1, 3].Value = "Promoción";
                worksheet.Cells[1, 4].Value = "Ciclo";
                worksheet.Cells[1, 5].Value = "Evento";
                worksheet.Cells[1, 6].Value = "Fecha Evento";
                worksheet.Cells[1, 7].Value = "Fecha Asistencia";

                using (var range = worksheet.Cells[1, 1, 1, 7])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                int row = 2;
                foreach (var asistencia in asistencias)
                {
                    worksheet.Cells[row, 1].Value = asistencia.Estudiante.Nombre;
                    worksheet.Cells[row, 2].Value = asistencia.Estudiante.Apellido;
                    worksheet.Cells[row, 3].Value = asistencia.Estudiante.Promocion.ToString();
                    worksheet.Cells[row, 4].Value = asistencia.Estudiante.Ciclo.ToString();
                    worksheet.Cells[row, 5].Value = asistencia.Evento.Nombre;
                    worksheet.Cells[row, 6].Value = asistencia.Evento.FechaHoraInicio.ToString("dd-MM-yyyy hh:mm tt");
                    worksheet.Cells[row, 7].Value = asistencia.FechaHoraRegistro.ToString("dd-MM-yyyy hh:mm tt");
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Asistencias-{DateTime.Now:dd-MM-yyyy}.xlsx";

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        // GET: Asistencia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia
                .Include(a => a.Administrador)
                .Include(a => a.Estudiante)
                .Include(a => a.Evento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // GET: Asistencia/Create
        public IActionResult Create()
        {
            ViewData["EventoId"] = new SelectList(_context.Evento.Where(e => e.Estado == Utils.Estado.ACTIVO), "Id", "Nombre");
            return View();
        }

        // POST: Asistencia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Asistencia asistencia)
        {
            ViewData["EventoId"] = new SelectList(_context.Evento.Where(e => e.Estado == Utils.Estado.ACTIVO), "Id", "Nombre");

            if (!string.IsNullOrEmpty(SearchEmailOrCode))
            {
                //https://learn.microsoft.com/es-es/dotnet/api/system.data.entity.queryableextensions.firstordefaultasync?view=entity-framework-6.2.0
                var estudiante = await _context.Estudiante.FirstOrDefaultAsync(e => e.Dni.Contains(SearchEmailOrCode) || e.CodigoEstudiante.Contains(SearchEmailOrCode));
                
                if (estudiante != null)
                {
                    bool asistenciaConsulta = _context.Asistencia.Any(a => a.EstudianteId == estudiante.Id && a.EventoId == asistencia.EventoId) == false;
                    if (asistenciaConsulta)
                    {
                        asistencia.EstudianteId = estudiante.Id;
                    }
                    else
                    {
                        _notify.Warning("Ya te as registrado a este evento");
                        return View(asistencia);
                    }
                }
                else
                {
                    _notify.Warning("No se ha encontrado al estudiante");
                    return View(asistencia);
                }
            }
            else
            {
                _notify.Error("Ingresa el código de estudiante o DNI");
                return View(asistencia);
            }

            ModelState.Remove("EstudianteId");
            ModelState.Remove("AdministradorId");

            if (ModelState.IsValid)
            {
                asistencia.FechaHoraRegistro = DateTime.Now;
                //==============NECESITAMOS PASARLE EL ID DEL ADMINISTRADOR================
                var administradorId = User.FindFirst("AdministradorId")?.Value;
                asistencia.AdministradorId = int.Parse(administradorId);
                _context.Add(asistencia);
                await _context.SaveChangesAsync();
                _notify.Success("Registro exitoso");
                //return RedirectToAction(nameof(Index));
            }

            return View(asistencia);
        }

        // GET: Asistencia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia.FindAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }
            ViewData["AdministradorId"] = new SelectList(_context.Administrador, "Id", "Apellido", asistencia.AdministradorId);
            ViewData["EstudianteId"] = new SelectList(_context.Estudiante, "Id", "Apellido", asistencia.EstudianteId);
            ViewData["EventoId"] = new SelectList(_context.Evento, "Id", "Nombre", asistencia.EventoId);
            return View(asistencia);
        }

        // POST: Asistencia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EstudianteId,EventoId,AdministradorId,FechaHoraRegistro")] Asistencia asistencia)
        {
            if (id != asistencia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asistencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsistenciaExists(asistencia.Id))
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
            ViewData["AdministradorId"] = new SelectList(_context.Administrador, "Id", "Apellido", asistencia.AdministradorId);
            ViewData["EstudianteId"] = new SelectList(_context.Estudiante, "Id", "Apellido", asistencia.EstudianteId);
            ViewData["EventoId"] = new SelectList(_context.Evento, "Id", "Nombre", asistencia.EventoId);
            return View(asistencia);
        }

        // GET: Asistencia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia
                .Include(a => a.Administrador)
                .Include(a => a.Estudiante)
                .Include(a => a.Evento)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // POST: Asistencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencia = await _context.Asistencia.FindAsync(id);
            if (asistencia != null)
            {
                _context.Asistencia.Remove(asistencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsistenciaExists(int id)
        {
            return _context.Asistencia.Any(e => e.Id == id);
        }
    }
}

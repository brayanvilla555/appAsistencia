using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectoAsistencia.Data;
using proyectoAsistencia.Models;
using proyectoAsistencia.Utils;
using X.PagedList;

namespace proyectoAsistencia.Controllers
{
    [Authorize]
    public class DocenteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;

        public DocenteController(AppDbContext context, IConfiguration configuration, INotyfService notyfService)
        {
            _context = context;
            _configuration = configuration;
            _notyfService = notyfService;
        }

        // GET: Docente
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Index(string? apellido, string nombre, int? page)
        {
            var docentes = _context.Docente.AsQueryable();

            if (!string.IsNullOrEmpty(apellido))
            {
                ViewData["BuscarApellido"] = apellido;
                docentes = docentes.Where(u => u.Apellido.Contains(apellido));
            }
            if (!string.IsNullOrEmpty(nombre))
            {
                ViewData["BuscarNombre"] = nombre;
                docentes = docentes.Where(u => u.Nombre.Contains(nombre));
            }

            int numeroPagina = page ?? 1;
            int tamañoPagina = _configuration.GetValue("RegistrosPorPagina", 15);

            return View(await docentes.OrderByDescending(e => e.Id).ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        [Authorize(Policy = "AdministradorPolicy")]
        public IActionResult LimpiarBusqueda()
        {
            return RedirectToAction("Index", new { nombre = (string?)null, apellido = (string?)null });
        }

        // GET: Docente/Details/5
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docente = await _context.Docente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docente == null)
            {
                return NotFound();
            }

            return View(docente);
        }

        // GET: Docente/Create
        [Authorize(Policy = "AdministradorPolicy")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Docente/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "AdministradorPolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Docente docente)
        {
            if (ModelState.IsValid)
            {
                var emailExists = await _context.Docente.AnyAsync(d => d.Email == docente.Email);
                if (emailExists)
                {
                    ModelState.AddModelError("Email", "El correo ya está registrado.");
                    return View(docente);
                }

                //https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.ipasswordhasher-1?view=aspnetcore-8.0
                var hasher = new PasswordHasher<Docente>();
                docente.Contrasena = hasher.HashPassword(docente, docente.Contrasena);

                _context.Add(docente);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(docente);
        }
        // GET: Docente/Edit/5
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docente = await _context.Docente.FindAsync(id);
            if (docente == null)
            {
                return NotFound();
            }
            return View(docente);
        }

        // POST: Docente/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Edit(int id, Docente docente)
        {
            if (id != docente.Id)
            {
                return NotFound();
            }
            ModelState.Remove("Contrasena");

            if (ModelState.IsValid)
            {
                try
                {
                    //https://learn.microsoft.com/es-es/dotnet/api/system.data.entity.queryableextensions.firstordefaultasync?view=entity-framework-6.2.0
                    var contrasenaOriginal = await _context.Docente
                        .Where(e => e.Id == id)
                        .Select(e => e.Contrasena)
                        .FirstOrDefaultAsync();

                    if (contrasenaOriginal == null)
                    {
                        return NotFound();
                    }

                    docente.Contrasena = contrasenaOriginal;

                    _context.Entry(docente).State = EntityState.Modified;

                    _context.Entry(docente).Property(e => e.Contrasena).IsModified = false;

                    await _context.SaveChangesAsync();

                    _notyfService.Success("Se editó el docente con éxito.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocenteExists(docente.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            _notyfService.Warning("Hubo un error al intentar editar el docente.");
            return View(docente);
        }



        // GET: Docente/Delete/5
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docente = await _context.Docente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (docente == null)
            {
                return NotFound();
            }

            return View(docente);
        }

        // POST: Docente/Delete/5
        [Authorize(Policy = "AdministradorPolicy")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var docente = await _context.Docente.FindAsync(id);
            if (docente != null)
            {
                _context.Docente.Remove(docente);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocenteExists(int id)
        {
            return _context.Docente.Any(e => e.Id == id);
        }

        //Concultar asistencias a los eventos
        [Authorize(Policy = "DocentePolicy")]
        public async Task<IActionResult> EventoAlumno(Ciclo? cicloAcademico, string eventoNombre, DateTime? searchDate, int ? page)

        {
            //var docenteId = User.FindFirst("DocenteId")?.Value;

            var consulta = _context.Asistencia.AsQueryable();

            if (cicloAcademico.HasValue)
            {
                ViewData["eventoSearchCiclo"] = cicloAcademico.Value;
                consulta = consulta.Where(u => u.Estudiante.Ciclo == cicloAcademico.Value);
            }

            if (searchDate.HasValue)
            {
                ViewData["searchDateValue"] = searchDate.Value.ToString("yyyy-MM-dd");
                consulta = consulta.Where(u => u.FechaHoraRegistro.Date == searchDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(eventoNombre))
            {
                ViewData["eventoSearchEvento"] = eventoNombre;
                consulta = consulta.Where(u => u.Evento.Nombre.Contains(eventoNombre.Trim()));
            }

            consulta = consulta.Include(u => u.Estudiante)
                               .Include(u => u.Evento)
                               .Include(u => u.Administrador)  as IQueryable<Asistencia>;

            //consulta = consulta.Where(e => e.Evento.FechaHoraFin.Date.Month == DateTime.Now.Date.Month);
            return View(await consulta.ToListAsync());
        }

        [Authorize(Policy = "DocentePolicy")]
        public IActionResult ClearSearchDocente()
        {
            return RedirectToAction("EventoAlumno", new { searchString = (string?)null });
        }

    }
}

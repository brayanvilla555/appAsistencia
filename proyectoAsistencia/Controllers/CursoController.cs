using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using proyectoAsistencia.Data;
using proyectoAsistencia.Models;
using X.PagedList;

namespace proyectoAsistencia.Controllers
{
    [Authorize(Policy = "AdministradorPolicy")]
    public class CursoController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;


        public CursoController(AppDbContext context, IConfiguration configuration, INotyfService notyfService)
        {
            _context = context;
            _configuration = configuration;
            _notyfService = notyfService;
        }

        // GET: Curso
        public async Task<IActionResult> Index(int? page, string busqueda)
        {
            var cursos = _context.Curso.AsQueryable();

            if (!string.IsNullOrEmpty(busqueda))
            {
                ViewData["valorBusquedaCadena"] = busqueda;
                cursos = cursos.Where(c => c.Nombre.Contains(busqueda));
            }

            int numeroPagina = page ?? 1;
            int tamañoPagina = _configuration.GetValue("RegistrosPorPagina", 15);

            return View(await cursos.OrderByDescending(e => e.Id).Include(c => c.Docentes).ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        // Limpiar búsqueda
        public IActionResult LimpiarBusqueda()
        {
            return RedirectToAction("Index", new { busqueda = (string?)null });
        }

        // GET: Curso/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Curso
                .Include(c => c.Docentes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // GET: Curso/Create
        public IActionResult Create()
        {
            ViewData["DocenteId"] = new SelectList(_context.Docente, "Id", "Apellido");
            return View();
        }

        // POST: Curso/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Curso curso)
        {
            var existeCursoDocente = await _context.Curso.AnyAsync(cd => cd.Nombre == curso.Nombre && cd.DocenteId == curso.DocenteId);

            if (existeCursoDocente)
            {
                var docente = await _context.Docente.FirstOrDefaultAsync(d => d.Id == curso.DocenteId);
                _notyfService.Error("El registro con este nombre ya existe.");
                ViewData["DocenteID"] = new SelectList(_context.Docente, "Id", "Apellido", curso.DocenteId);
                return View(curso);
            }

            if (ModelState.IsValid)
            {
                _context.Add(curso);
                await _context.SaveChangesAsync();
                _notyfService.Success("Se guardo exitosamente");
                return RedirectToAction(nameof(Index));
            }

            ViewData["DocenteID"] = new SelectList(_context.Docente, "Id", "Apellido", curso.DocenteId);
            _notyfService.Warning("Verifique los datos");

            return View(curso);
        }

        // GET: Curso/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Curso.FindAsync(id);
            if (curso == null)
            {
                return NotFound();
            }
            ViewData["DocenteId"] = new SelectList(_context.Docente, "Id", "Apellido", curso.DocenteId);
            return View(curso);
        }

        // POST: Curso/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,ciclo,DocenteId")] Curso curso)
        {
            if (id != curso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(curso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CursoExists(curso.Id))
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
            ViewData["DocenteId"] = new SelectList(_context.Docente, "Id", "Apellido", curso.DocenteId);
            return View(curso);
        }

        // GET: Curso/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var curso = await _context.Curso
                .Include(c => c.Docentes)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (curso == null)
            {
                return NotFound();
            }

            return View(curso);
        }

        // POST: Curso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _context.Curso.FindAsync(id);
            if (curso != null)
            {
                _context.Curso.Remove(curso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CursoExists(int id)
        {
            return _context.Curso.Any(e => e.Id == id);
        }
    }
}

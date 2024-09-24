using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectoAsistencia.Data;
using proyectoAsistencia.Models;
using X.PagedList;

namespace proyectoAsistencia.Controllers
{
    [Authorize(Policy = "AdministradorPolicy")]
    public class PonenteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notyfService;
        private readonly IConfiguration _configuration;
        public PonenteController(AppDbContext context, INotyfService notyfService, IConfiguration configuration)
        {
            _context = context;
            _notyfService = notyfService;
            _configuration = configuration;
        }

        // GET: Ponente
        public async Task<IActionResult> Index(string? cadenaBusqueda, int ? page)
        {
            var consulta = _context.Ponente.AsQueryable();

            if (!string.IsNullOrWhiteSpace(cadenaBusqueda))
            {
                ViewData["cadenaBusquedaValor"] = cadenaBusqueda;
                consulta = consulta.Where(p => p.Nombre.Contains(cadenaBusqueda) || p.Apellido.Contains(cadenaBusqueda));
            }
            int numeroPagina = page ?? 1;
            int tamañoPagina = _configuration.GetValue("RegistrosPorPagina", 15);

            return View(await consulta.OrderByDescending(e => e.Id).ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        // Limpiar la búsqueda
        [HttpGet]
        public IActionResult LimpiarBusqueda()
        {
            return RedirectToAction("Index", new { cadenaBusqueda = (string?)null });
        }

        // GET: Ponente/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ponente = await _context.Ponente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ponente == null)
            {
                return NotFound();
            }

            return View(ponente);
        }

        // GET: Ponente/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Ponente/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Apellido,Especializacion,Email,Telefono")] Ponente ponente)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ponente);
                await _context.SaveChangesAsync();
                _notyfService.Success("Se creó con éxito");
                return RedirectToAction(nameof(Index));
            }
            return View(ponente);
        }

        // GET: Ponente/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ponente = await _context.Ponente.FindAsync(id);
            if (ponente == null)
            {
                return NotFound();
            }
            return View(ponente);
        }

        // POST: Ponente/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Apellido,Especializacion,Email,Telefono")] Ponente ponente)
        {
            if (id != ponente.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ponente);
                    _notyfService.Success("Se edito con éxito");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PonenteExists(ponente.Id))
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
            return View(ponente);
        }

        // GET: Ponente/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ponente = await _context.Ponente
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ponente == null)
            {
                return NotFound();
            }

            return View(ponente);
        }

        // POST: Ponente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ponente = await _context.Ponente.FindAsync(id);
            if (ponente != null)
            {
                _context.Ponente.Remove(ponente);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Se eliminó con éxito");
            return RedirectToAction(nameof(Index));
        }

        private bool PonenteExists(int id)
        {
            return _context.Ponente.Any(e => e.Id == id);
        }
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using proyectoAsistencia.Data;
using proyectoAsistencia.Models;
using X.PagedList;

namespace proyectoAsistencia.Controllers
{

    //[Authorize]
    //https://es.stackoverflow.com/questions/111076/c%C3%B3mo-proteger-accesos-a-las-p%C3%A1ginas-en-mvc
    [Authorize(Policy = "AdministradorPolicy")]

    public class AdministradorController : Controller
    {
        private readonly INotyfService _notyf;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AdministradorController(AppDbContext context, INotyfService notyf, IConfiguration configuration)
        {
            _context = context;
            _notyf = notyf;
            _configuration = configuration;
        }


        public IActionResult Dashboard()
        {
            return View();
        }

        // GET: Administrador
        public async Task<IActionResult> Index(string? cadenaBusqueda, int? page)
        {
            var consulta = _context.Administrador.AsQueryable();

            if (!string.IsNullOrEmpty(cadenaBusqueda))
            {
                ViewData["ValorCadenaBusqueda"] = cadenaBusqueda;
                consulta = consulta.Where(a => a.Nombre.Contains(cadenaBusqueda) || a.Apellido.Contains(cadenaBusqueda));
            }

            int numeroPagina = page ?? 1;
            int tamañoPagina = _configuration.GetValue("RegistrosPorPagina", 15);

            return View(await consulta.OrderByDescending(e => e.Id).ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        public IActionResult LimpiarBusqueda()
        {

            return RedirectToAction("Index", new { cadenaBusqueda = (string?)null });
        }

        // GET: Administrador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administrador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // GET: Administrador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Administrador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Administrador administrador)
        {
            if (ModelState.IsValid)
            {
                var emailExists = await _context.Administrador.AnyAsync(a => a.Email == administrador.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "El correo ya está registrado.");
                    _notyf.Warning("El correo ya está registrado");
                    return View(administrador);
                }

                // Encriptar la contraseña
                var hasher = new PasswordHasher<Administrador>();
                administrador.Contrasena = hasher.HashPassword(administrador, administrador.Contrasena);

                _context.Add(administrador);
                await _context.SaveChangesAsync();

                _notyf.Success("Se registró con éxito");
                return RedirectToAction(nameof(Index));
            }

            _notyf.Warning("Hubo un error al intentar registrar");
            return View(administrador);
        }

        // GET: Administrador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administrador.FindAsync(id);
            if (administrador == null)
            {
                return NotFound();
            }
            return View(administrador);
        }

        // POST: Administrador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Administrador administrador)
        {
            if (id != administrador.Id)
            {
                return NotFound();
            }

            var emailExists = await _context.Administrador
                .Where(a => a.Id != administrador.Id) 
                .AnyAsync(a => a.Email == administrador.Email);

            if (emailExists)
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
                _notyf.Warning("El correo electrónico ya está registrado.");
                return View(administrador);
            }

            if (ModelState.IsValid)
            {
                var administradorId = User.FindFirst("AdministradorId")?.Value;

                if (id != int.Parse(administradorId))
                {
                    _notyf.Warning("No puedes editar la contraseña ni los datos de otro administrador.");
                    return View(administrador);
                }

                try
                {
                    var hasher = new PasswordHasher<Administrador>();

                    var currentAdmin = await _context.Administrador.AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Id == id);


                    if (currentAdmin == null)
                    {
                        return NotFound();
                    }

                    if (currentAdmin.Contrasena != administrador.Contrasena)
                    {
                        administrador.Contrasena = hasher.HashPassword(administrador, administrador.Contrasena);
                    }

                    _context.Update(administrador);
                    await _context.SaveChangesAsync();
                    _notyf.Success("Se editó con éxito.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdministradorExists(administrador.Id))
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

            _notyf.Warning("Hubo un error al editar el registro.");
            return View(administrador);
        }


        // GET: Administrador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var administrador = await _context.Administrador
                .FirstOrDefaultAsync(m => m.Id == id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View(administrador);
        }

        // POST: Administrador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var administrador = await _context.Administrador.FindAsync(id);
            if (administrador != null)
            {
                _context.Administrador.Remove(administrador);
            }

            _notyf.Success("Se eliminó con exito");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdministradorExists(int id)
        {
            return _context.Administrador.Any(e => e.Id == id);
        }
    }
}

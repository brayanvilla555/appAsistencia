using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using proyectoAsistencia.Data;
using proyectoAsistencia.Models;
using OfficeOpenXml;
using proyectoAsistencia.Utils;
using X.PagedList;
using AspNetCoreHero.ToastNotification.Abstractions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace proyectoAsistencia.Controllers
{
    [Authorize]
    public class EstudianteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;

        public EstudianteController(AppDbContext context, IConfiguration configuration, INotyfService notyfService)
        {
            _context = context;
            _configuration = configuration;
            _notyfService = notyfService;
        }

        // GET: Estudiante
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Index(string? cadenaBusqueda, Ciclo? buscarCiclo, int? page)
        {
            int numeroPagina = page ?? 1;
            int tamañoPagina = _configuration.GetValue("RegistrosPorPagina", 15);

            var consulta = _context.Estudiante.AsQueryable();
            if (!string.IsNullOrWhiteSpace(cadenaBusqueda))
            {
                ViewData["ValorCadenaBusqueda"] = cadenaBusqueda;
                consulta = consulta.Where(e => e.CodigoEstudiante.Contains(cadenaBusqueda));
            }

            if (buscarCiclo.HasValue)
            {
                ViewData["BuscarCiclo"] = buscarCiclo.Value;
                consulta = consulta.Where(e => e.Ciclo == buscarCiclo.Value);
            }

            return View(await consulta.OrderByDescending(e => e.Id).ToPagedListAsync(numeroPagina, tamañoPagina));
        }

        [Authorize(Policy = "AdministradorPolicy")]
        public IActionResult LimpiarBusqueda()
        {
            return RedirectToAction("Index", new { cadenaBusqueda = (string?)null });
        }


        // GET: Estudiante/Details/5
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //esplicacion 
            //https://learn.microsoft.com/es-es/ef/core/querying/related-data/eager
            var estudiante = await _context.Estudiante.FirstOrDefaultAsync(m => m.Id == id);

            if (estudiante == null)
            {
                return NotFound();
            }

            return View(estudiante);
        }

        // GET: Estudiante/Create
        [Authorize(Policy = "AdministradorPolicy")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Estudiante/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "AdministradorPolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Estudiante model)
        {
            ModelState.Remove("Contrasena");

            if (ModelState.IsValid)
            {
                var codigoExists = await _context.Estudiante.AnyAsync(e => e.CodigoEstudiante == model.CodigoEstudiante);
                var dniExists = await _context.Estudiante.AnyAsync(e => e.Dni == model.Dni);
                var emailExists = await _context.Estudiante.AnyAsync(e => e.Email == model.Email);

                if (codigoExists)
                {
                    _notyfService.Warning("El código del estudiante ya está registrado.");
                    return View(model);
                }
                if (dniExists)
                {
                    _notyfService.Warning("El DNI ya está registrado.");
                    return View(model);
                }
                if (emailExists)
                {
                    _notyfService.Warning("El correo ya está registrado.");
                    return View(model);
                }

                model.Contrasena = model.CodigoEstudiante.Trim() + "_unc_" + DateTime.Now.Year;

                var hasher = new PasswordHasher<Estudiante>();
                model.Contrasena = hasher.HashPassword(model, model.Contrasena);

                _context.Estudiante.Add(model);
                await _context.SaveChangesAsync();
                _notyfService.Success("Se guardó exitosamente.");
                return RedirectToAction("Index");
            }

            return View(model);
        }



        //importa desde excel
        [Authorize(Policy = "AdministradorPolicy")]
        [HttpPost]
        public async Task<IActionResult> ImportarExcel(IFormFile archivoExcel)
        {
            if (archivoExcel != null && archivoExcel.Length > 0)
            {
                using (var stream = new MemoryStream())
                {
                    await archivoExcel.CopyToAsync(stream);
                    using (var paquete = new ExcelPackage(stream))
                    {
                        var hojaTrabajo = paquete.Workbook.Worksheets.First();
                        var totalFilas = hojaTrabajo.Dimension.Rows;

                        for (int fila = 2; fila <= totalFilas; fila++)
                        {
                            try
                            {
                                var codigoEstudiante = hojaTrabajo.Cells[fila, 1].Text?.Trim();
                                var dni = hojaTrabajo.Cells[fila, 2].Text?.Trim();
                                var nombre = hojaTrabajo.Cells[fila, 3].Text?.Trim();
                                var apellido = hojaTrabajo.Cells[fila, 4].Text?.Trim();
                                var promocion = hojaTrabajo.Cells[fila, 5].Text?.Trim();

                                int indiceCiclo;
                                if (!int.TryParse(hojaTrabajo.Cells[fila, 6].Text?.Trim(), out indiceCiclo) || indiceCiclo < 0 || indiceCiclo > 9)
                                {
                                    _notyfService.Warning($"Fila {fila}: El ciclo es inválido o está fuera de rango.");
                                    continue; 
                                }

                                var telefono = hojaTrabajo.Cells[fila, 7].Text?.Trim();
                                var correoElectronico = hojaTrabajo.Cells[fila, 8].Text?.Trim();

                                var contraseñaGenerada = codigoEstudiante + "_unc_" + DateTime.Now.Year;

                                var hasher = new PasswordHasher<Estudiante>();
                                var estudiante = new Estudiante
                                {
                                    CodigoEstudiante = codigoEstudiante,
                                    Dni = dni,
                                    Nombre = nombre,
                                    Apellido = apellido,
                                    Promocion = promocion,
                                    Ciclo = (Ciclo)indiceCiclo,  
                                    Telefono = telefono,
                                    Email = correoElectronico,
                                    Contrasena = hasher.HashPassword(null, contraseñaGenerada) 
                                };

                                var contextoValidacion = new ValidationContext(estudiante, null, null);
                                var resultadosValidacion = new List<ValidationResult>();
                                bool esValido = Validator.TryValidateObject(estudiante, contextoValidacion, resultadosValidacion, true);

                                if (!esValido)
                                {
                                    foreach (var resultadoValidacion in resultadosValidacion)
                                    {
                                        _notyfService.Warning($"Fila {fila}: {resultadoValidacion.ErrorMessage}");
                                    }
                                    continue;
                                }

                                _context.Estudiante.Add(estudiante);
                            }
                            catch (Exception ex)
                            {
                                _notyfService.Warning($"Fila {fila}: Ocurrió un error al procesar los datos. {ex.Message}");
                            }
                        }

                        try
                        {
                            await _context.SaveChangesAsync();
                            _notyfService.Success("Se importó exitosamente.");
                        }
                        catch (Exception ex)
                        {
                            _notyfService.Error($"Error al guardar los datos: {ex.Message}");
                        }
                    }
                }
            }
            else
            {
                _notyfService.Warning("Por favor, sube un archivo válido.");
            }

            return RedirectToAction(nameof(Index));
        }




        // GET: Estudiante/Edit/5
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudiante = _context.Estudiante
                            .FirstOrDefault(e => e.Id == id);

            if (estudiante == null)
            {
                return NotFound();
            }

            return View(estudiante);
        }

        // POST: Estudiante/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "AdministradorPolicy")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Estudiante model)
        {
            ModelState.Remove("Contrasena");

            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var estudianteActual = await _context.Estudiante.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);

                if (estudianteActual == null)
                {
                    return NotFound();
                }

                var codigoExists = await _context.Estudiante.AnyAsync(e => e.CodigoEstudiante == model.CodigoEstudiante && e.Id != id);
                var dniExists = await _context.Estudiante.AnyAsync(e => e.Dni == model.Dni && e.Id != id);
                var emailExists = await _context.Estudiante.AnyAsync(e => e.Email == model.Email && e.Id != id);

                if (codigoExists)
                {
                    _notyfService.Warning("El código del estudiante ya está registrado.");
                    return View(model);
                }
                if (dniExists)
                {
                    _notyfService.Warning("El DNI ya está registrado.");
                    return View(model);
                }
                if (emailExists)
                {
                    _notyfService.Warning("El correo ya está registrado.");
                    return View(model);
                }
                model.Contrasena = estudianteActual.Contrasena;

                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Se editó exitosamente.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Estudiante.AnyAsync(e => e.Id == model.Id))
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

            return View(model);
        }


        // GET: Estudiante/Delete/5
        [Authorize(Policy = "AdministradorPolicy")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var estudiante = await _context.Estudiante
                .FirstOrDefaultAsync(m => m.Id == id);
            if (estudiante == null)
            {
                return NotFound();
            }

            return View(estudiante);
        }

        // POST: Estudiante/Delete/5
        [Authorize(Policy = "AdministradorPolicy")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var estudiante = await _context.Estudiante.FindAsync(id);
            if (estudiante != null)
            {
                _context.Estudiante.Remove(estudiante);
            }

            await _context.SaveChangesAsync();
            _notyfService.Success("Se eliminó con éxito");
            return RedirectToAction(nameof(Index));
        }

        private bool EstudianteExists(int id)
        {
            return _context.Estudiante.Any(e => e.Id == id);
        }

        [Authorize(Policy = "EstudiantePolicy")]
        public async Task<IActionResult> MiAsistencia()
        {
            //sacamos el id de la sesion
            var studentId = User.FindFirst("EstudianteId")?.Value;

            var query = _context.Asistencia.AsQueryable();
            query = query.OrderByDescending(a => a.Id).Include(a => a.Evento).Where(a => a.EstudianteId == int.Parse(studentId));

            return View(await query.ToListAsync());
        }
    }
}

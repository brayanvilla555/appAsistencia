using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using proyectoAsistencia.Data;
using System.Security.Claims;
using proyectoAsistencia.ViewModel;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace proyectoAsistencia.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notyfService;

        //https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-6.0&tabs=visual-studio
        public LoginController(AppDbContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _notyfService.Success("Verificando credenciales...");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ClaimsIdentity identity = null;
            bool isAuthenticated = false;

            var hasher = new PasswordHasher<object>();

            if (model.Role == "Administrador")
            {
                var admin = await _context.Administrador.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (admin != null)
                {
                    var resultadoVerificacion = hasher.VerifyHashedPassword(null, admin.Contrasena, model.Password);
                    if (resultadoVerificacion == PasswordVerificationResult.Success)
                    {
                        identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, admin.Email),
                    new Claim(ClaimTypes.Role, "Administrador"),
                    new Claim("AdministradorId", admin.Id.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                        isAuthenticated = true;
                    }
                }
            }
            else if (model.Role == "Estudiante")
            {
                var estudiante = await _context.Estudiante.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (estudiante != null)
                {
                    var resultadoVerificacion = hasher.VerifyHashedPassword(null, estudiante.Contrasena, model.Password);
                    if (resultadoVerificacion == PasswordVerificationResult.Success)
                    {
                        identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, estudiante.Email),
                    new Claim(ClaimTypes.Role, "Estudiante"),
                    new Claim("EstudianteId", estudiante.Id.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                        isAuthenticated = true;
                    }
                }
            }
            else if (model.Role == "Docente")
            {
                var docente = await _context.Docente.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (docente != null)
                {
                    var resultadoVerificacion = hasher.VerifyHashedPassword(null, docente.Contrasena, model.Password);
                    if (resultadoVerificacion == PasswordVerificationResult.Success)
                    {
                        identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, docente.Email),
                    new Claim(ClaimTypes.Role, "Docente"),
                    new Claim("DocenteId", docente.Id.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                        isAuthenticated = true;
                    }
                }
            }

            if (isAuthenticated)
            {
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (model.Role == "Administrador")
                {
                    return RedirectToAction("Dashboard", "Administrador");
                }
                else if (model.Role == "Docente")
                {
                    return RedirectToAction("EventoAlumno", "Docente");
                }
                else
                {
                    return RedirectToAction("MiAsistencia", "Estudiante");
                }
            }

            _notyfService.Warning("Correo, contraseña o usuario equivocado");
            return RedirectToAction("Index", "Home");
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

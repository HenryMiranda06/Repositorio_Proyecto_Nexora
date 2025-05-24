using Microsoft.AspNetCore.Mvc;
using Sistema_RRHH_Nexora.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Sistema_RRHH_Nexora.Controllers
{
    public class LoginController : Controller
    {
        private static ConsumoAPI _consumo;
        private HttpClient client;

        public LoginController()
        {
            _consumo = new ConsumoAPI();
            client = _consumo.Iniciar();

        }

        [HttpGet]
        public IActionResult SolicitarCuenta()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarCuenta(SolicitudesCuenta solicitud)
        {
            try
            {
                //de momento para pruebas
                solicitud.NoSolicitud = 0;
                solicitud.Contraseña = GenerarClave();

                if (!ModelState.IsValid)
                {
                    return View(solicitud);
                }

                var response = await client.PutAsJsonAsync($"/api/Cuentas/VerificarSolicitud", solicitud);
                string mensaje = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode) //significa que el codigo devolvió 200 OK
                {
                    Email email = new Email();
                    await email.EnviarEmail(solicitud, 3);
                }
                TempData["Mensaje"] = mensaje;
                return View(solicitud);
            }
            catch (Exception)
            {
                TempData["Mensaje"] = "No se logra conectar con el servidor";
                return View(solicitud);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Cuentas cuenta)
        {
            try
            {
                //cuenta.ID_Empleado = 0;

                var response = await client.PutAsJsonAsync("/api/Cuentas/VerificarCuenta", cuenta);
                var datos = await response.Content.ReadFromJsonAsync<DTO_DatosCuenta>();

                if (response.IsSuccessStatusCode)
                {
                    var credencialesUsuario = new List<Claim>() { new Claim(ClaimTypes.Name, datos.Cuenta.Correo),
                        new Claim(ClaimTypes.Role, datos.NombreRol) };

                    var identidadUsuario = new ClaimsIdentity(credencialesUsuario, "User Identity");

                    var entidadUsuario = new ClaimsPrincipal(new[] { identidadUsuario });

                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme); //cerrar sesión anterior, por seguridad

                    await HttpContext.SignInAsync(entidadUsuario);

                    return RedirectToAction("Login", "Login");
                }

                TempData["Mensaje"] = datos.Mensaje;
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                TempData["Mensaje"] = "Error al conectar con el servidor.";
                return RedirectToAction("Index", "Home");
            }
        }

        //Desloguearse
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //Generamos una clave temporal para el login
        public string GenerarClave()
        {
            Random rnd = new Random();

            string clave = string.Empty;

            //caracteres utilizados para generar la clave
            clave = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZ0123456789";

            //se genera la clave aleatoria retornando su valor
            return new string(Enumerable.Repeat(clave, 12).Select
                (s => s[rnd.Next(s.Length)]).ToArray());
        }
    }
}

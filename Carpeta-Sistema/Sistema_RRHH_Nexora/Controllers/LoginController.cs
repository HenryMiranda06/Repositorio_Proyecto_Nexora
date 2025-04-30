using Microsoft.AspNetCore.Mvc;
using Sistema_RRHH_Nexora.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Net;
using Microsoft.IdentityModel.Tokens;

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
            //de momento para pruebas
            solicitud.NoSolicitud = 0;
            solicitud.Contraseña = "default";

            Dictionary<string, char> existe = await ExisteSolicitud(solicitud);
            string mensaje = existe.Keys.First();

            if (!ModelState.IsValid)
            {
                return View(solicitud);
            }

            if (existe.Values.Contains('E'))
            {
                TempData["Mensaje"] = mensaje;
                return View(solicitud);
            }
            else if (existe.Values.Contains('N')) //se guarda la solicitud de crear cuenta
            {
                var subir = await client.PutAsJsonAsync("/api/Cuentas/SolicitarCuenta", solicitud);

                if (subir.IsSuccessStatusCode)
                {
                    TempData["Exito"] = await subir.Content.ReadAsStringAsync();
                    return RedirectToAction("SolicitarCuenta", "Login");
                }
            }
            else
            {
                TempData["Mensaje"] = mensaje;
                return View(solicitud);
            }

            return View(solicitud);
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
            if (string.IsNullOrEmpty(cuenta.Correo) || string.IsNullOrEmpty(cuenta.Contraseña))
            {
                TempData["Mensaje"] = "Ambos campos deben estar llenos";
                return RedirectToAction("Index", "Home");
            }

            cuenta.ID_Empleado = 0;

            var response = await client.PutAsJsonAsync("/api/Cuentas/VerificarCuenta", cuenta);

            if (response.IsSuccessStatusCode)
            {
                var existe = await response.Content.ReadFromJsonAsync<char>();

                if (existe == 'e')
                {
                    if (await VerificarRol(cuenta) == 'e')
                    {
                        var credencialesCuenta = new List<Claim>
                        {
                             new Claim(ClaimTypes.Name, cuenta.Correo),
                             new Claim(ClaimTypes.Role, "RRHH")
                        };

                        var granIdentity = new ClaimsIdentity(credencialesCuenta, "User Identity");
                        var userPrincipal = new ClaimsPrincipal(new[] { granIdentity });

                        await HttpContext.SignInAsync(userPrincipal);
                        return RedirectToAction("Login", "Login");
                    }
                    else
                    {
                        TempData["Mensaje"] = "No tienes permisos para acceder.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    TempData["Mensaje"] = "Error, el correo o contraseña es incorrecto.";
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["Mensaje"] = "Error al conectar con el servidor.";
            return RedirectToAction("Index", "Home");
        }

        //Desloguearse
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<Dictionary<string, char>> ExisteSolicitud(SolicitudesCuenta cuenta)
        {
            Dictionary<string, char> respuesta = new Dictionary<string, char>();

            var response = await client.PutAsJsonAsync($"/api/Cuentas/VerificarSolicitud", cuenta);

            string mensaje = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                respuesta.Add(mensaje, 'N'); //No existe solicitud con ese correo
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                respuesta.Add(mensaje, 'E'); //existe solicitud
            }
            else
            {
                respuesta.Add(mensaje, 'F'); //error de la API
            }

            return respuesta;
        }

        //Metodo para ver si la cuenta que se va a loguear es de recursos humanos
        public async Task<char> VerificarRol(Cuentas cuenta)
        {
            char respuesta = 'f';

            var response = await client.PutAsJsonAsync($"/api/Roles/RolesEmpleado", cuenta);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                respuesta = 'e';
            }

            return respuesta;
        }
    }
}

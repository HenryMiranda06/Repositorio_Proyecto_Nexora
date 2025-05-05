using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Newtonsoft.Json;
using Sistema_RRHH_Nexora.Models;

namespace Sistema_RRHH_Nexora.Controllers
{
    public class SolicitudesController : Controller
    {
        private static ConsumoAPI _apiCLient;
        private HttpClient client;
        Email email = new Email();

        public SolicitudesController()
        {
            _apiCLient = new ConsumoAPI();
            client = _apiCLient.Iniciar();
        }


        public async Task<IActionResult> Solicitudes() //Lista de solicitudes
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("RRHH"))
                {
                    List<SolicitudesCuenta> lista = new List<SolicitudesCuenta>();

                    HttpResponseMessage response = await client.GetAsync("/api/Solicitudes/Listado");

                    if (response.IsSuccessStatusCode)
                    {
                        var resultado = response.Content.ReadAsStringAsync().Result;

                        lista = JsonConvert.DeserializeObject<List<SolicitudesCuenta>>(resultado);

                        return View(lista);

                    }
                }
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Solicitudes(int id, string accion)
        {
            string mensaje = "";
            var objeto = await RetornarSolicitud(id);

            if (accion == "aceptar")
            {
                mensaje = await AceptarSolicitud(id);
                if (!mensaje.Equals("Error")) //si funcionó
                {
                    await email.EnviarEmail(objeto, 1);
                    return RedirectToAction("Solicitudes", "Solicitudes");

                }
                else
                {
                    return View(Solicitudes());
                }
            }
            else
            {
                mensaje = await RechazarSolicitud(id);
                if (!mensaje.Equals("Error"))
                {
                    await email.EnviarEmail(objeto, 2);
                    return RedirectToAction("Solicitudes", "Solicitudes");
                }
                else
                {
                    return View(Solicitudes());
                }
            }
        }

        public async Task<string> AceptarSolicitud(int id)
        {
            var response = await client.PutAsync($"/api/Solicitudes/AceptarSolicitud/{id}", null);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return "Error";
        }

        public async Task<string> RechazarSolicitud(int id)
        {
            var response = await client.DeleteAsync($"/api/Solicitudes/RechazarSolicitud/{id}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return "Error";
        }

        public async Task<SolicitudesCuenta> RetornarSolicitud(int id)
        {
            SolicitudesCuenta solicitud = new SolicitudesCuenta();

            var response = await client.GetAsync($"/api/Solicitudes/BuscarSolicitud/{id}");

            if (response.IsSuccessStatusCode)
            {
                var objeto = await response.Content.ReadAsStringAsync();
                solicitud = JsonConvert.DeserializeObject<SolicitudesCuenta>(objeto);
            }
            return solicitud;
        }
    }
}

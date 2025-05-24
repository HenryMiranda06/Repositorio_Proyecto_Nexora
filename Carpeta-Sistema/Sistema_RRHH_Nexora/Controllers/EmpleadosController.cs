using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Sistema_RRHH_Nexora.Models;

namespace Sistema_RRHH_Nexora.Controllers
{
    public class EmpleadosController : Controller
    {
        private static ConsumoAPI _apiCLient;
        private HttpClient client;

        public EmpleadosController()
        {
            _apiCLient = new ConsumoAPI();
            client = _apiCLient.Iniciar();
        }



        // GET: EmpleadosController
        public async Task<IActionResult> Index() //Lista de empleados
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("RRHH"))
                {
                    List<DTO_ListaEmpleados> lista = new List<DTO_ListaEmpleados>();

                    var response = await client.GetAsync("/api/Empleados/Listado");

                    if (response.IsSuccessStatusCode)
                    {
                        var resultado = await response.Content.ReadAsStringAsync();

                        lista = JsonConvert.DeserializeObject<List<DTO_ListaEmpleados>>(resultado);

                        return View(lista);

                    }
                }
            }
            return View();
        }

        //Metodos get y post para crear el empleado
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.Roles = await ListaRoles();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Empleados empleado, int ID_Rol)
        {
            if (ID_Rol == 0)
            {
                ModelState.AddModelError("ID_Rol", "Debe seleccionar un puesto de trabajo.");

                ViewBag.Roles = await ListaRoles();

                return View(empleado);
            }

            var respuesta = await BuscarEmpleado(empleado.Cedula);

            //primero traemos de la vista
            if (respuesta.Item2 == 'E') //si el empleado ya existe
            {
                TempData["Mensaje"] = respuesta.Item1;
                ViewBag.Roles = await ListaRoles();
                return View(empleado);
            }
            else if (respuesta.Item2 == 'N')//si no existe, guardar en la BD
            {
                empleado.ID_Empleado = 0;

                var agregarEmpleado = await client.PutAsJsonAsync($"/api/Empleados/Agregar/{ID_Rol}", empleado);

                if (agregarEmpleado.IsSuccessStatusCode) //si se agregó a la BD con éxito
                {
                    return RedirectToAction("Index"); //envia a la lista de Empleados
                }
                else
                {
                    ViewBag.Roles = await ListaRoles();
                    return View(empleado);
                }
            }
            else
            {
                TempData["Mensaje"] = respuesta.Item1;
                ViewBag.Roles = await ListaRoles();
                return View(empleado);
            }
        }

        //Verifica si el empleado a ingresar existe, mediante su cedula
        public async Task<(string, char)> BuscarEmpleado(int cedula)
        {
            var response = await client.GetAsync($"/api/Empleados/Buscar/{cedula}");
            var mensaje = await response.Content.ReadAsStringAsync();

            return response.StatusCode switch
            {
                HttpStatusCode.NotFound => ("", 'N'),
                HttpStatusCode.OK => (mensaje, 'E'),
                HttpStatusCode.BadRequest => (mensaje, 'F')
            };
        }

        public async Task<List<Roles>> ListaRoles()
        {
            var response = await client.GetAsync("/api/Roles/Listado");

            List<Roles> lista = new List<Roles>();

            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadAsStringAsync();

                lista = JsonConvert.DeserializeObject<List<Roles>>(resultado);
            }

            return lista;
        }
    }
}

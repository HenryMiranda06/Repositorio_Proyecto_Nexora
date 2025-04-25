using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            List<Empleados> lista = new List<Empleados>();

            HttpResponseMessage response = await client.GetAsync("/api/Empleados/Agregar");

            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;

                lista = JsonConvert.DeserializeObject<List<Empleados>>(resultado);
            }

            return View(lista);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmpleadosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Empleados empleado)
        {
            //primero traemos de la vista
            if (await BuscarEmpleado(empleado.Cedula)) //si el empleado ya existe
            {
                TempData["Mensaje"] = "El empleado que desea ingresar ya existe en el sistema.";
                return RedirectToAction("Create", "Empleados");
            }
            else //si no existe, guardar en la BD
            {
                var agregarEmpleado = await client.PutAsJsonAsync<Empleados>("/api/Empleados/Agregar", empleado);

                if (agregarEmpleado.IsSuccessStatusCode) //si se agregó a la BD con éxito
                {
                    return RedirectToAction("Index"); //envia a la lista de Empleados
                }
                else
                {
                    return View(empleado);
                }
            }
        }

        // GET: EmpleadosController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EmpleadosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmpleadosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmpleadosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //Metodo buscar Empleado de la API
        public async Task<bool> BuscarEmpleado(int cedula)
        {
            Empleados empleado = new Empleados();
            bool empleadoExiste = false;

            var response = await client.GetAsync($"/api/Empleados/{cedula}");

            if (response.IsSuccessStatusCode)
            {
                var json = response.Content.ReadAsStringAsync().Result;

                empleado = JsonConvert.DeserializeObject<Empleados>(json);

                if (empleado != null)
                {
                    empleadoExiste = true;
                }
            }

            return empleadoExiste;
        }
    }
}

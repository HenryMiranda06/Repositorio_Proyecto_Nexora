using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_NEXORA_INGSW.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API_NEXORA_INGSW.Controllers
{
    [Route("api/Empleados")]
    [ApiController]
    public class EmpleadosController : Controller
    {
        private readonly DbContextNexora _context;

        private CuentasController _cuentasController;

        private RolesController _rolesController;

        private PerfilProfesionalController _perfilProfesionalController;

        private IdiomasController _idiomasController;

        private readonly ConcursosController _concursosController;

        public EmpleadosController(DbContextNexora context)
        {
            _context = context;
            _cuentasController = new CuentasController(_context);
            _rolesController = new RolesController(_context);
            _perfilProfesionalController = new PerfilProfesionalController(_context);
            _idiomasController = new IdiomasController(_context);
            _concursosController = new ConcursosController(_context);
        }

        [HttpPut("Agregar/{id}")]
        public async Task<IActionResult> Agregar(Empleados empleado, int id)
        {
            using (var transaccion = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    empleado.ID_Empleado = await GenerarCodigo();

                    _context.Empleados.Add(empleado);

                    //agregar el puesto de trabajo
                    _context.RolesEmpleado.Add(new RolesEmpleado { ID_Empleado = empleado.ID_Empleado, ID_Rol = id});

                    await _context.SaveChangesAsync();

                    if (await _perfilProfesionalController.CrearPerfil(empleado.ID_Empleado))
                    {
                        await transaccion.CommitAsync();
                        //se indica un mensaje de exito

                    }
                    return Ok("Empleado ingresado correctamente");
                }
                catch (Exception e)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest("Ocurrió un error inesperado: " + e.Message.ToString());
                }
            }
        }

        [HttpGet("Listado")]
        public async Task<List<Empleados>> Listado()
        {
            //se lee la lista de empleados en la base datos
            var lista = await _context.Empleados.ToListAsync();

            return lista; //se retorna la lista de empleados
        }

        [HttpGet("Buscar/{cedula}")]
        public async Task<IActionResult> Buscar(int cedula)
        {
            try
            {
                //se busca el empleado
                var temp = await _context.Empleados.FirstOrDefaultAsync(x => x.Cedula == cedula);

                if (temp != null) //si existe
                {
                    return Ok("Ya existe un empleado con ese número de cedula");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e.InnerException.Message);
            }
        }

        //Método para realizar el proceso de modificación de datos
        [HttpPost("Modificar")]
        public async Task<string> Modificar(Empleados empleado)
        {
            string mensaje = $"No se logró aplicar los cambios al  empleado {empleado.NombreEmpleado}\n";
            try
            {
                _context.Empleados.Update(empleado);
                await _context.SaveChangesAsync();
                mensaje = $"Los cambios al empleado {empleado.NombreEmpleado} se aplicaron correctamente..";
            }
            catch (Exception ex)
            {

                mensaje += "Error, " + ex.InnerException.Message;
            }

            return mensaje;
        }

        //Método que elimina a un empleado en base a su numero de cedula
        [HttpDelete("Eliminar/{cedula, id}")]
        public async Task<string> Eliminar(int cedula, int id)
        {
            string mensaje = "Error inesperado";

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //busca al empleado en la BD
                    var empleado = await _context.Empleados.FirstOrDefaultAsync(x => x.Cedula == cedula
                    && x.ID_Empleado == id);

                    //si existe
                    if (empleado != null)
                    {
                        //Elimina los datos asociados al empleado (cuentas, solicitudes, roles, perfil, idiomas, postulaciones)
                        var cuentas = await _context.Cuentas.Where(r => r.ID_Empleado == id).ToListAsync();

                        //devuelve true si trae algun elemento al menos
                        if (cuentas.Any())
                        {
                            //elimina todos los que trajo
                            _context.Cuentas.RemoveRange(cuentas);

                            // Guardar cambios después de eliminar las cuentas
                            await _context.SaveChangesAsync();
                        }

                        await EliminarDatosEmpleado(id);

                        //elimina el empleado
                        _context.Empleados.Remove(empleado);

                        // Guardar cambios después de eliminar el empleado
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        mensaje = "Empleado: " + empleado.NombreEmpleado + " elimnado exitosamente";
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    mensaje = "No se eliminó el empleado. Error: " + (ex.InnerException?.Message);
                }
            }

            return mensaje;
        }

        [HttpDelete]
        public async Task EliminarDatosEmpleado(int id)
        {
            await _cuentasController.EliminarSolicitudID(id);  //eliminar solicitudes cuenta del empleado

            await _rolesController.EliminarRolesEmpleado(id); //eliminar roles del empleado

            await _idiomasController.EliminarIdiomasEmpleado(id); //eliminar Idiomas del empleado

            await _concursosController.EliminarParticipante(id); //elimina al empleado de todos los concursos

            await _perfilProfesionalController.EliminarPerfil(id); //elimina el perfil profesional del empleado
        }


        [HttpPost]
        //Metodo que genera un codigo de empleado de 4 digitos de forma random
        public async Task<int> GenerarCodigo()
        {
            Random combinacion = new Random();

            int min = 1000;
            int max = 9999;
            int codigo = 0;
            do
            {
                codigo = combinacion.Next(min, max + 1);
            } while (await _context.Empleados.AnyAsync(e => e.ID_Empleado == codigo));

            return codigo;
        }
    }
}

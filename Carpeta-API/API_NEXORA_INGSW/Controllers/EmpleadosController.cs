using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_NEXORA_INGSW.Models;

namespace API_NEXORA_INGSW.Controllers
{
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

        [HttpPut("Agregar")]
        public async Task<string> Agregar(Empleados empleado)
        {
            //
            string mensaje = "";
            try
            {
                //se realiza la transacción utilizando el  ORM
                _context.Empleados.Add(empleado);

                //se aplican los cambios
                await _context.SaveChangesAsync();

                //se indica un mensaje de exito
                mensaje = $"Empleado {empleado.NombreEmpleado} fue agregado correctamente...";

            }
            catch (Exception ex)
            {

                mensaje = "Error " + ex.InnerException.Message;
            }
            return mensaje;
        }

        [HttpGet("Listado")]
        public async Task<List<Empleados>> Listado()
        {
            //se lee la lista de clientes en la base datos
            var lista = await _context.Empleados.ToListAsync();

            return lista; //se retorna la lista de clientes
        }

        [HttpGet("Buscar/{cedula}")]
        public async Task<Empleados> Buscar(int cedula)
        {
            //se busca el cliente
            var temp = await _context.Empleados.FirstOrDefaultAsync(x => x.Cedula == cedula);

            return temp;
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

                        await _cuentasController.EliminarSolicitudID(id);  //eliminar solicitudes cuenta del empleado

                        await _rolesController.EliminarRolesEmpleado(id); //eliminar roles del empleado

                        await _idiomasController.EliminarIdiomasEmpleado(id); //eliminar Idiomas del empleado

                        await _concursosController.EliminarParticipante(id); //elimina al empleado de todos los concursos

                        await _perfilProfesionalController.EliminarPerfil(id); //elimina el perfil profesional del empleado

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
    }
}

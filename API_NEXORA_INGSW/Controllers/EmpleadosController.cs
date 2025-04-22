using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_NEXORA_INGSW.Models;

namespace API_NEXORA_INGSW.Controllers
{
    public class EmpleadosController : Controller
    {
        private readonly DbContextNexora _context;

        public EmpleadosController(DbContextNexora context)
        {
            _context = context;
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
        [HttpDelete("Eliminar/{cedula}")]
        public async Task<string> Eliminar(int cedula)
        {
            string mensaje = "No se eliminó el empleado.";

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                /*try
                {
                    var cliente = await _context.Clientes.FirstOrDefaultAsync(x => x.Cedula == cedula);
                    if (cliente != null)
                    {
                        //todas las reservaciones con ese cliente
                        var reservaciones = await _context.Reservaciones
                            .Where(r => r.CedulaCliente == cedula)
                            .ToListAsync();

                        //devuelve true si trae algun elemento al menos
                        if (reservaciones.Any())
                        {
                            //elimina todos los que trajo
                            _context.Reservaciones.RemoveRange(reservaciones);

                            // Guardar cambios después de eliminar las reservaciones
                            await _context.SaveChangesAsync();
                        }

                        //elimina el cliente
                        _context.Clientes.Remove(cliente);

                        // Guardar cambios después de eliminar el cliente
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        mensaje = "Cliente " + cliente.NombreCompleto + " y sus reservaciones eliminados correctamente.";
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    mensaje = "No se eliminó el cliente. Error: " + (ex.InnerException?.Message ?? ex.Message);
                }*/
            }

            return mensaje;
        }
    }
}

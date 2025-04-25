using Microsoft.AspNetCore.Mvc;
using API_NEXORA_INGSW.Models;
using Microsoft.EntityFrameworkCore;

namespace API_NEXORA_INGSW.Controllers
{
    [Route("api/Cuentas")]
    [ApiController]
    public class CuentasController : Controller
    {
        private readonly DbContextNexora _context;

        public CuentasController(DbContextNexora context)
        {
            _context = context;
        }

        [HttpPut("SolicitarCuenta")]
        public async Task<string> SolicitudCrearCuenta(SolicitudesCuenta solicitud)
        {
            string mensaje = "";

            try
            {
                _context.Solicitudes.Add(solicitud);

                await _context.SaveChangesAsync();

                mensaje = "Se envió su solicitud exitosamente." +
                    "\nEspere la respuesta de RRHH";
            }
            catch (Exception e)
            {
                mensaje = "Error " + e.InnerException.Message;
            }

            return mensaje;
        }

        [HttpPut("CrearCuenta")]
        //Metodo crear cuenta
        public async Task<string> CrearCuentaEmpleado(SolicitudesCuenta cuentaEmp)
        {
            string mensaje = "Error inesperado";

            try
            {
                Cuentas cuentas = new Cuentas
                {
                    Correo = cuentaEmp.CorreoCuenta,
                    Contraseña = cuentaEmp.Contraseña,
                    ID_Empleado = cuentaEmp.ID_Empleado
                };

                _context.Cuentas.Add(cuentas);
                await _context.SaveChangesAsync();

                mensaje = "Cuenta creada exitosamente";

            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.InnerException.Message;
            }
            return mensaje;
        }

        [HttpDelete("EliminarSoli/{id}")]
        //Eliminar solicitud de cuenta
        public async Task<string> EliminarSolicitud(int id)
        {
            string mensaje = "Error inesperado";

            try
            {
                var cuentaEliminar = await _context.Solicitudes.FirstOrDefaultAsync(
                    t => t.NoSolicitud == id);

                if (cuentaEliminar != null)
                {
                    _context.Solicitudes.Remove(cuentaEliminar);

                    await _context.SaveChangesAsync();

                    mensaje = "Se rechazó la solicitud de cuenta";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.InnerException.Message;
            }
            return mensaje;
        }

        //Eliminar soli pero buscando por id empleado
        [HttpDelete("EliminarSoliID/{id}")]
        public async Task EliminarSolicitudID(int id)
        {
            try
            {
                var cuentaEliminar = await _context.Solicitudes.FirstOrDefaultAsync(
                    t => t.ID_Empleado == id);

                if (cuentaEliminar != null)
                {
                    _context.Solicitudes.Remove(cuentaEliminar);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }
        }
    }
}

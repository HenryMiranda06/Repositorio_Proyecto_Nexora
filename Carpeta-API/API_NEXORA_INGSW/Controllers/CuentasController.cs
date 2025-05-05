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
            string mensaje = "Error inesperado";

            try
            {
                //traer el siguiente ID para la solicitud
                int sigID = await _context.SolicitudesCuenta.CountAsync();

                solicitud.NoSolicitud = sigID + 1;

                _context.SolicitudesCuenta.Add(solicitud);

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

        //verifica si existe una solicitud con un correo específico
        [HttpPut("VerificarSolicitud")]
        public async Task<IActionResult> VerificarSolicitud(SolicitudesCuenta solicitud)
        {
            try
            {
                var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.ID_Empleado == solicitud.ID_Empleado);

                if (empleado == null)
                {
                    return NotFound("El código es inválido.");
                }

                var credenciales = await _context.SolicitudesCuenta.FirstOrDefaultAsync(
               t => t.ID_Empleado == solicitud.ID_Empleado || t.CorreoCuenta.Equals(solicitud.CorreoCuenta));

                if (credenciales != null) //si ya hay una solicitud
                {
                    return Unauthorized("Ya existe una solicitud pendiente de aprobación.");
                }
                else
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return BadRequest("Ocurrió un error inesperado");

            }
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
        public async Task<IActionResult> EliminarSolicitud(int id)
        {
            try
            {
                var cuentaEliminar = await _context.SolicitudesCuenta.FirstOrDefaultAsync(
                    t => t.NoSolicitud == id);

                if (cuentaEliminar != null)
                {
                    _context.SolicitudesCuenta.Remove(cuentaEliminar);

                    await _context.SaveChangesAsync();

                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return BadRequest();
        }

        //Eliminar soli pero buscando por id empleado
        [HttpDelete("EliminarSoliID/{id}")]
        public async Task EliminarSolicitudID(int id)
        {
            try
            {
                var cuentaEliminar = await _context.SolicitudesCuenta.FirstOrDefaultAsync(
                    t => t.ID_Empleado == id);

                if (cuentaEliminar != null)
                {
                    _context.SolicitudesCuenta.Remove(cuentaEliminar);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
            }
        }

        [HttpPut("VerificarCuenta")]
        public async Task<char> VerificarCredenciales([FromBody] Cuentas credenciales)
        {
            char respuesta = 'f';

            var cuenta = await _context.Cuentas.
                FirstOrDefaultAsync(t => t.Correo == credenciales.Correo &&
                t.Contraseña == credenciales.Contraseña);

            if (cuenta != null)
            {
                respuesta = 'e';
            }
            else
            {
                respuesta = 'n';
            }

            return respuesta;
        }
    }
}

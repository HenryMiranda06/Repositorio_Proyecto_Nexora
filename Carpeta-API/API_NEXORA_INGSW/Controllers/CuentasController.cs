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

        private async Task<bool> SolicitudCrearCuenta(SolicitudesCuenta solicitud)
        {
            try
            {
                //traer el siguiente ID para la solicitud
                int sigID = await _context.SolicitudesCuenta.CountAsync();

                solicitud.NoSolicitud = sigID + 1;

                _context.SolicitudesCuenta.Add(solicitud);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //verifica si existe una solicitud con un correo específico
        [HttpPut("VerificarSolicitud")]
        public async Task<IActionResult> VerificarSolicitud(SolicitudesCuenta solicitud)
        {
            try
            {
                var empleado = await _context.Empleados.FirstOrDefaultAsync(x => x.ID_Empleado == solicitud.ID_Empleado); //buscamos empleado
                if (empleado == null)
                {
                    return BadRequest("Código de empleado inválido");
                }

                var cuenta = await _context.Cuentas.FirstOrDefaultAsync(x => x.ID_Empleado == solicitud.ID_Empleado); //cuenta por idEmpleado
                if (cuenta != null)
                {
                    return BadRequest("El empleado ya posee una cuenta");
                }

                var existeSolicitud = await _context.SolicitudesCuenta.FirstOrDefaultAsync(x => x.ID_Empleado == solicitud.ID_Empleado); //solicitud por idEmpleado
                if (existeSolicitud != null)
                {
                    return BadRequest("Ya tiene una solicitud de creación de cuenta. Espere su respuesta");
                }

                var correoCuenta = await _context.Cuentas.FirstOrDefaultAsync(x => x.Correo == solicitud.CorreoCuenta); //buscar cuenta por correo
                if (correoCuenta != null)
                {
                    return BadRequest("El correo indicado se encuentra en uso, utilice uno distinto");
                }

                var correoSolicitud = await _context.SolicitudesCuenta.FirstOrDefaultAsync(x => x.CorreoCuenta == solicitud.CorreoCuenta); //buscar solicitud por correo
                if (correoSolicitud != null)
                {
                    return BadRequest("Ya existe una solicitud con ese correo, utilice uno distinto");
                }

                if (await SolicitudCrearCuenta(solicitud))
                {
                    return Ok("Solicitud procesada con éxito");
                }
                return BadRequest("Error inesperado");
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
        public async Task<IActionResult> VerificarCredenciales([FromBody] Cuentas credenciales)
        {
            try
            {
                var existeCuenta = await _context.Cuentas.FirstOrDefaultAsync(x => x.Correo == credenciales.Correo
                && x.Contraseña == credenciales.Contraseña);

                if (existeCuenta != null)
                {
                    var rol = await _context.RolesEmpleado.FirstOrDefaultAsync(x => x.ID_Empleado == credenciales.ID_Empleado);
                    var nombreRol = await _context.Roles.FirstOrDefaultAsync(x => x.ID_Rol == rol.ID_Rol);

                    var dto = new DTO_DatosCuenta
                    {
                        Cuenta = new Cuentas
                        {
                            ID_Empleado = credenciales.ID_Empleado,
                            Correo = credenciales.Correo,
                            Contraseña = credenciales.Contraseña
                        },
                        IdRol = rol.ID_Rol,
                        NombreRol = nombreRol.NombreRol
                    };

                    return Ok(new { mensaje = "Credenciales", datos = dto });
                }
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos" });
            }
            catch (Exception)
            {
                return BadRequest(new { mensaje = "Ocurrió un error inesperado" });
            }
        }
    }
}

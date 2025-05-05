using API_NEXORA_INGSW.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_NEXORA_INGSW.Controllers
{
    [Route("api/Solicitudes")]
    [ApiController]
    public class SolicitudesController : Controller
    {
        private readonly DbContextNexora _context;

        public SolicitudesController(DbContextNexora context)
        {
            _context = context;
        }

        [HttpGet("Listado")]
        public async Task<List<SolicitudesCuenta>> Listado()
        {
            //se lee la lista de solicitudes en la base datos
            var lista = await _context.SolicitudesCuenta.ToListAsync();

            return lista; //se retorna la lista de solicitudes
        }

        [HttpGet("BuscarSolicitud/{id}")]
        public async Task<SolicitudesCuenta> BuscarSolicitud(int id)
        {
            try
            {
                var temp = await _context.SolicitudesCuenta.FirstOrDefaultAsync(
                    t => t.NoSolicitud == id);

                if (temp != null)
                {
                    return temp;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        [HttpPut("AceptarSolicitud/{id}")]
        public async Task<IActionResult> AceptarSolicitud(int id)
        {
            using (var transaccion = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var datosSolicitud = await _context.SolicitudesCuenta.FirstOrDefaultAsync(
                        d => d.NoSolicitud == id);

                    if (datosSolicitud != null)
                    {
                        Cuentas cuenta = new Cuentas
                        {
                            Correo = datosSolicitud.CorreoCuenta,
                            Contraseña = datosSolicitud.Contraseña,
                            ID_Empleado = datosSolicitud.ID_Empleado
                        };

                        _context.Cuentas.Add(cuenta);

                        await _context.SaveChangesAsync();

                        //eliminamos la solicitud
                        _context.SolicitudesCuenta.Remove(datosSolicitud);

                        await _context.SaveChangesAsync();

                    }
                    //termina la transaccion
                    await transaccion.CommitAsync();

                    return Ok("Cuenta creada con éxito.");
                }
                catch (Exception e)
                {
                    await transaccion.RollbackAsync();
                    return BadRequest("Error: " + e.Message);
                }
            }
        }

        [HttpDelete("RechazarSolicitud/{id}")]
        public async Task<IActionResult> RechazarSolicitud(int id)
        {
            try
            {
                var datosSolicitud = await _context.SolicitudesCuenta.FirstOrDefaultAsync(
                        d => d.NoSolicitud == id);

                if (datosSolicitud != null)
                {
                    _context.SolicitudesCuenta.Remove(datosSolicitud);

                    await _context.SaveChangesAsync();

                }
                return Ok("Solicitud eliminada con éxito");
            }
            catch (Exception e)
            {
                return BadRequest("Error: " + e.Message);
            }
        }
    }
}

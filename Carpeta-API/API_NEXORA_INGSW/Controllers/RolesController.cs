using Microsoft.AspNetCore.Mvc;
using API_NEXORA_INGSW.Models;
using Microsoft.EntityFrameworkCore;

namespace API_NEXORA_INGSW.Controllers
{
    [Route("api/Roles")]
    [ApiController]
    public class RolesController : Controller
    {
        private readonly DbContextNexora _context;

        public RolesController(DbContextNexora context)
        {
            _context = context;
        }

        [HttpGet("Listado")]
        public async Task<List<Roles>> listadoRoles()
        {
            var roles = await _context.Roles.ToListAsync();

            return roles;
        }


        //Eliminar TODOS los roles de un empleado
        [HttpDelete("EliminarTodosRoles")]
        public async Task EliminarRolesEmpleado(int idEmpleado)
        {
            var roles = await _context.RolesEmpleado.Where(r => r.ID_Empleado == idEmpleado).ToListAsync();

            if (roles != null)
            {
                _context.RolesEmpleado.RemoveRange(roles);

                await _context.SaveChangesAsync();
            }
        }

        [HttpPut("RolesEmpleado")]
        public async Task<ActionResult<bool>> VerificarRolRRHH(Cuentas cuenta)
        {
            if (cuenta == null || string.IsNullOrEmpty(cuenta.Correo) || string.IsNullOrEmpty(cuenta.Contraseña))
                return BadRequest("Datos de cuenta incompletos.");

            var cuentaEmpleado = await _context.Cuentas
                .FirstOrDefaultAsync(c => c.Correo == cuenta.Correo && c.Contraseña == c.Contraseña);

            if (cuentaEmpleado == null)
                return Unauthorized("Cuenta no encontrada o credenciales incorrectas.");

            var rolesEmpleado = await _context.RolesEmpleado
                .Where(r => r.ID_Empleado == cuentaEmpleado.ID_Empleado)
                .ToListAsync();

            var rolRRHH = await _context.Roles
                .FirstOrDefaultAsync(r => r.NombreRol == "RRHH");

            if (rolRRHH == null)
                return StatusCode(500, "El rol 'RRHH' no está definido en el sistema.");

            bool tieneRolRRHH = rolesEmpleado.Any(r => r.ID_Rol == rolRRHH.ID_Rol);

            return Ok(tieneRolRRHH); // Devuelve true o false, pero con 200 OK
        }
    }
}

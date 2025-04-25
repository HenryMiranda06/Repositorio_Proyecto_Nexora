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
    }
}

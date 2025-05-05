using Microsoft.AspNetCore.Mvc;
using API_NEXORA_INGSW.Models;
using Microsoft.EntityFrameworkCore;

namespace API_NEXORA_INGSW.Controllers
{
    [Route("api/Idiomas")]
    [ApiController]
    public class IdiomasController : Controller
    {
        private readonly DbContextNexora _context;
        public IdiomasController(DbContextNexora context)
        {
            _context = context;
        }

        [HttpDelete("EliminarIdiomasEmpleado/{idEmpleado}")]
        public async Task EliminarIdiomasEmpleado(int idEmpleado)
        {
            try
            {
                var perfil = await _context.PerfilProfesional.FirstOrDefaultAsync(
                    p => p.ID_Empleado == idEmpleado);

                if (perfil != null)
                {
                    var idiomasEmpleado = await _context.IdiomasEmpleado.Where(
                    i => i.ID_Perfil == perfil.ID_Perfil).ToListAsync();

                    if (idiomasEmpleado != null)
                    {
                        _context.IdiomasEmpleado.RemoveRange(idiomasEmpleado);

                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        //Muestra todos los idiomas
        [HttpGet("ListadoIdiomas")]
        public async Task<List<Idiomas>> ListadoIdiomas()
        {
            var lista = await _context.Idiomas.ToListAsync();

            return lista;
        }
    }
}

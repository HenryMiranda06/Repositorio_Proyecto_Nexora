using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_NEXORA_INGSW.Models;

namespace API_NEXORA_INGSW.Controllers
{
    public class ConcursosController : Controller
    {
        private readonly DbContextNexora _context;

        public ConcursosController(DbContextNexora context)
        {
            _context = context;
        }

        //Métodos
        [HttpPut("CrearConcurso")]
        public async Task<string> CrearConcurso(Concursos concurso)
        {
            string mensaje = "Error inesperado";
            try
            {
                _context.Concursos.Add(concurso);

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                mensaje = "Error " + ex.InnerException.Message;
            }
            return mensaje;
        }

        [HttpDelete("EliminarConcurso")]
        public async Task<string> EliminarConcurso(Concursos concurso)
        {
            string mensaje = "Error inesperado";

            try
            {
                var buscarConcurso = await _context.Concursos.FirstOrDefaultAsync(t => t.ID_Concurso == concurso.ID_Concurso);

                if (buscarConcurso != null)
                {
                    _context.Concursos.Remove(buscarConcurso);

                    await _context.SaveChangesAsync();

                    mensaje = "Concurso eliminado";
                }
                else
                {
                    mensaje = "No se encontró el concurso";
                }
            }
            catch (Exception ex)
            {
                mensaje = "Error: " + ex.InnerException.Message;
            }

            return mensaje;
        }

    }
}

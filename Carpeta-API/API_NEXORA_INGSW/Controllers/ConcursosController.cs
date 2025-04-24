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
        public async Task<string> CrearConcurso(Concursos concurso, RequisitosConcurso requisitos)
        {
            string mensaje = "Error inesperado";

            using (var transaccion = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Concursos.Add(concurso);

                    await _context.SaveChangesAsync();

                    ////////////////////////////////////////

                    _context.RequisitosConcurso.Add(requisitos);

                    await _context.SaveChangesAsync();

                    await transaccion.CommitAsync();
                }
                catch (Exception)
                {
                    await transaccion.RollbackAsync();
                }
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

        //Elimina un empleado de todos los concursos
        [HttpDelete("EliminarParticipante/{idEmpleado}")]
        public async Task EliminarParticipante(int idEmpleado)
        {
            try
            {
                var perfil = await _context.PerfilProfesional.FirstOrDefaultAsync(
                    p => p.ID_Empleado == idEmpleado);

                if (perfil != null)
                {
                    var postulante = await _context.Postulaciones.Where(
                    i => i.ID_Perfil == perfil.ID_Perfil).ToListAsync();

                    if (postulante != null)
                    {
                        _context.Postulaciones.RemoveRange(postulante);

                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

    }
}

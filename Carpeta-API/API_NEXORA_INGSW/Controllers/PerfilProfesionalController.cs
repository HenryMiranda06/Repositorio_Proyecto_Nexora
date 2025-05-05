using Microsoft.AspNetCore.Mvc;
using API_NEXORA_INGSW.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_NEXORA_INGSW.Controllers
{
    [Route("api/PerfilProf")]
    [ApiController]
    public class PerfilProfesionalController : Controller
    {
        private readonly DbContextNexora _context;

        public PerfilProfesionalController(DbContextNexora context)
        {
            _context = context;
        }

        [HttpPut]
        public async Task<bool> CrearPerfil(int idEmpleado)
        {
            try
            {
                int idPerfil = 1 + await _context.PerfilProfesional.CountAsync();

                DTO_PerfilProfesional datosPerfil = new DTO_PerfilProfesional
                {

                    perfil = new PerfilProfesional
                    {
                        ID_Perfil = idPerfil,
                        ID_Empleado = idEmpleado,
                        HistorialLaboral = "Default",
                        Certificados = new byte[0],
                        Titulos = new byte[0]
                    },
                    idiomas = new IdiomasEmpleado
                    {
                        Cod_Idioma = "df",
                        ID_Perfil = idPerfil
                    }
                };

                //llenar tabla perfil profesional
                _context.PerfilProfesional.Add(datosPerfil.perfil);

                await _context.SaveChangesAsync();

                _context.IdiomasEmpleado.Add(datosPerfil.idiomas);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("EROR: " + e.Message);
            }
            return false;
        }


        [HttpDelete("EliminarPerfil/{idEmpleado}")]
        public async Task<string> EliminarPerfil(int idEmpleado)
        {
            string mensaje = "Error inesperado";

            try
            {
                var perfil = await _context.PerfilProfesional.FirstOrDefaultAsync(
                    t => t.ID_Empleado == idEmpleado);

                if (perfil != null)
                {
                    _context.PerfilProfesional.Remove(perfil);

                    await _context.SaveChangesAsync();

                    mensaje = "Error al eliminar el perfil profesional";
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

﻿using Microsoft.AspNetCore.Mvc;
using API_NEXORA_INGSW.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API_NEXORA_INGSW.Controllers
{
    public class PerfilProfesionalController : Controller
    {
        private readonly DbContextNexora _context;

        public PerfilProfesionalController(DbContextNexora context)
        {
            _context = context;
        }

        //Metodos 
        [HttpPut("CrearPerfil")]
        public async Task<string> CrearPerfil(PerfilProfesional datosPerfil, IdiomasEmpleado idiomas)
        {
            string mensaje = "Error inesperado";

            using (var transaccion = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //llenar tabla perfil profesional
                    _context.PerfilProfesional.Add(datosPerfil);

                    await _context.SaveChangesAsync();

                    _context.IdiomasEmpleado.Add(idiomas);

                    await _context.SaveChangesAsync();

                    await transaccion.CommitAsync();

                    mensaje = "Perfil profesional creado con exito";
                }
                catch (Exception ex)
                {
                    await transaccion.RollbackAsync();
                    mensaje = "Error: " + ex.InnerException.Message;
                }
            }
            return mensaje;
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

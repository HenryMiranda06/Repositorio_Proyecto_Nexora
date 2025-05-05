using Microsoft.EntityFrameworkCore;

namespace API_NEXORA_INGSW.Models
{
    public class DbContextNexora : DbContext
    {
        public DbContextNexora(DbContextOptions<DbContextNexora> options) : base(options)
        {

        }

        public DbSet<Empleados> Empleados { get; set; }
        public DbSet<Concursos> Concursos { get; set; }
        public DbSet<Cuentas> Cuentas { get; set; }
        public DbSet<Idiomas> Idiomas { get; set; }
        public DbSet<IdiomasEmpleado> IdiomasEmpleado { get; set; }
        public DbSet<PerfilProfesional> PerfilProfesional { get; set; }
        public DbSet<Postulaciones> Postulaciones { get; set; }
        public DbSet<Requisitos> Requisitos { get; set; }
        public DbSet<RequisitosConcurso> RequisitosConcurso { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<RolesEmpleado> RolesEmpleado { get; set; }

        public DbSet<SolicitudesCuenta> SolicitudesCuenta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdiomasEmpleado>()
                .HasKey(t => new { t.Cod_Idioma, t.ID_Perfil });

            modelBuilder.Entity<RolesEmpleado>()
                .HasKey(t => new { t.ID_Empleado, t.ID_Rol });
        }
    }
}

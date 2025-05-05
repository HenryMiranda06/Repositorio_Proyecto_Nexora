using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class Roles
    {
        [Key]
        public int ID_Rol {  get; set; }

        public string NombreRol { get; set; }
    }
}

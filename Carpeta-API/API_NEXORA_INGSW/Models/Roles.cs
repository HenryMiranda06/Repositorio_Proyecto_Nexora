using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class Roles
    {
        [Key]
        public int ID_Rol {  get; set; }

        public string NombreRol { get; set; }
    }
}

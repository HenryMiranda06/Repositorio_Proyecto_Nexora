using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class Cuentas
    {
        [Key]
        public string Correo { get; set; }

        public string Contraseña { get; set; }

        public int ID_Empleado { get; set; }
    }
}

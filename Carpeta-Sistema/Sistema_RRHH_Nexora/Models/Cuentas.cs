using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class Cuentas
    {
        [Key]
        public string Correo { get; set; }

        public string Contraseña { get; set; }

        public int ID_Empleado { get; set; }
    }
}

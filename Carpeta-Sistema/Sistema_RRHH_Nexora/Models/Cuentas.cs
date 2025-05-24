using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class Cuentas
    {
        [Key]
        [Required(ErrorMessage = "Debe ingresar un correo electrónico")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "Debe ingresar la contraseña")]
        public string Contraseña { get; set; }

        public int ID_Empleado { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class SolicitudesCuenta
    {
        [Key]
        [Display(Name = "Número de solicitud")]
        public int? NoSolicitud { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [Display(Name = "Código de empleado")]
        public int ID_Empleado { get; set; }

        [Required(ErrorMessage = "Este campo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        [Display(Name = "Correo electrónico")]
        public string CorreoCuenta { get; set; }

        public string? Contraseña { get; set; }
    }
}

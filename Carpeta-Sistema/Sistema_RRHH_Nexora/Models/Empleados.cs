using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class Empleados
    {
        //Atributos
        [Key]
        [Display(Name = "ID Empleado")]
        public int? ID_Empleado { get; set; }

        [Display(Name = "Nombre del empleado")]
        [Required]
        public string NombreEmpleado { get; set; }

        [Required]
        public int Cedula { get; set; }

        [Required]
        public string Pais { get; set; }
    }
}

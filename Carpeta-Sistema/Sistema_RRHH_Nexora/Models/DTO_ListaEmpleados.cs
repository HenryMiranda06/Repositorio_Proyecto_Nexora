using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class DTO_ListaEmpleados
    {

        [Key]
        [Display(Name = "ID Perfil")]
        public int idPerfil { get; set; }

        public Empleados lista { get; set; }

        [Display(Name = "Correo")]
        public string correoEmp { get; set; }
    }
}

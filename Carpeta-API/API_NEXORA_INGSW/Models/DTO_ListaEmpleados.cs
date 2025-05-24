using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class DTO_ListaEmpleados
    {
        [Key]
        public int idPerfil { get; set; }

        public Empleados lista {  get; set; }

        public string correoEmp {  get; set; }
    }
}

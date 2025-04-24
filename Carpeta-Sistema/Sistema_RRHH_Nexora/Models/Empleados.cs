using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class Empleados
    {
        //Atributos
        [Key]
        public int ID_Empleado { get; set; }

        public string NombreEmpleado { get; set; }

        public int Cedula { get; set; } 

        public string Pais { get; set; }
    }
}

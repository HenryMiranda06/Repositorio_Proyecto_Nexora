using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class Postulaciones
    {
        [Key]
        public int ID_Postulaciones {  get; set; }

        public int ID_Concurso { get; set; }

        public int ID_Perfil { get; set; }

        public DateTime FechaPostulacion { get; set; }

        public char Estado { get; set; }
    }
}

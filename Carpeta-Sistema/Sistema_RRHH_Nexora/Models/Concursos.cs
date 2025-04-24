using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class Concursos
    {
        [Key]
        public int ID_Concurso { get; set; }

        public int ID_Rol {  get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaPublicacion { get; set; }

        public char Estado {  get; set; }

        public DateTime FechaCierre { get; set; }

        public int Encargado { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class RequisitosConcurso
    {
        [Key]
        public int ID_RequisitosConcurso { get; set; }

        public int ID_Concurso { get; set; }

        public int ID_Requisito {  get; set; }

        public string Valor {  get; set; }
    }
}

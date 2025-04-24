using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class Idiomas
    {
        [Key]
        public string Cod_Idioma { get; set; }

        public string Idioma { get; set; }
    }
}

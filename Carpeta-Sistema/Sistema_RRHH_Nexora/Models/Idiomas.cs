using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class Idiomas
    {
        [Key]
        public string Cod_Idioma { get; set; }

        public string Idioma { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class Requisitos
    {
        [Key]
        public int ID_Requisito { get; set; }

        public string NombreRequisito { get; set; }

        public string Descripcion { get; set; }
    }
}

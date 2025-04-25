using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
{
    public class Requisitos
    {
        [Key]
        public int ID_Requisito { get; set; }

        public string NombreRequisito { get; set; }

        public string Descripcion { get; set; }
    }
}

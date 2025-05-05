using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class PerfilProfesional
    {
        [Key]
        public int ID_Perfil {  get; set; }

        public byte[] Titulos { get; set; }

        public byte[] Certificados { get; set; }

        public string HistorialLaboral { get; set; }

        public int ID_Empleado { get; set; }
    }
}

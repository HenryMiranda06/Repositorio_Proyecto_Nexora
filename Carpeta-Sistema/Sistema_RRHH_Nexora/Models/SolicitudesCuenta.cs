using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class SolicitudesCuenta
    {
        [Key]
        public int NoSolicitud { get; set; }

        public int ID_Empleado { get; set; }

        public string CorreoCuenta { get; set; }

        public string Contraseña { get; set; }
    }
}

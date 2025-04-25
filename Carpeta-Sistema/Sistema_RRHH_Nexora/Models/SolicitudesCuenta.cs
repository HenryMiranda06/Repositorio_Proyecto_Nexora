using System.ComponentModel.DataAnnotations;

namespace Sistema_RRHH_Nexora.Models
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

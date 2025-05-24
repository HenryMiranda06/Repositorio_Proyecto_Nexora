using System.ComponentModel.DataAnnotations;

namespace API_NEXORA_INGSW.Models
{
    public class DTO_DatosCuenta
    {
        public Cuentas Cuenta { get; set; }

        public int IdRol { get; set; }

        public string NombreRol { get; set; }
    }
}

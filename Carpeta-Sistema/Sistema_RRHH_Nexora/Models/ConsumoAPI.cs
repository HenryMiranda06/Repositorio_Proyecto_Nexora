namespace Sistema_RRHH_Nexora.Models
{
    public class ConsumoAPI
    {
        public HttpClient Iniciar()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:7012");

            return client;
        }
    }
}

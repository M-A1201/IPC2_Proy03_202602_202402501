using System.Text.Json.Serialization;

namespace ServicioFrontend.Models
{
    public class ResumenIvaDto
    {
        [JsonPropertyName("nit")]
        public string Nit { get; set; } = "";

        [JsonPropertyName("ivaEmitido")]
        public decimal IvaEmitido { get; set; }

        [JsonPropertyName("ivaRecibido")]
        public decimal IvaRecibido { get; set; }
    }
}
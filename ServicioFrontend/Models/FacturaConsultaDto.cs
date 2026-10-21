using System.Text.Json.Serialization;

namespace ServicioFrontend.Models
{
    public class FacturaConsultaDto
    {
        [JsonPropertyName("TIEMPO")]
        public string Tiempo { get; set; } = "";

        [JsonPropertyName("REFERENCIA")]
        public string Referencia { get; set; } = "";

        [JsonPropertyName("NIT EMISOR")]
        public string NitEmisor { get; set; } = "";

        [JsonPropertyName("NIT RECEPTOR")]
        public string NitReceptor { get; set; } = "";

        [JsonPropertyName("VALOR")]
        public decimal Valor { get; set; }

        [JsonPropertyName("IVA")]
        public decimal Iva { get; set; }

        [JsonPropertyName("TOTAL")]
        public decimal Total { get; set; }
    }
}
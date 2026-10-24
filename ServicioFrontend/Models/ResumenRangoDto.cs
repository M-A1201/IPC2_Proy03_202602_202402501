using System.Text.Json.Serialization;

namespace ServicioFrontend.Models
{
    public class ResumenRangoDto
    {
        [JsonPropertyName("fecha")]
        public string Fecha { get; set; } = "";

        [JsonPropertyName("cantidadFacturas")]
        public int CantidadFacturas { get; set; }

        [JsonPropertyName("monto")]
        public decimal Monto { get; set; }
    }
}
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ServicioFrontend.Services
{
    public class ApiServicio
    {
        private readonly HttpClient _httpClient;

        public ApiServicio(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Aquí apuntamos al puerto donde corre tu ServicioAPI (vimos que era el 5220)
            _httpClient.BaseAddress = new Uri("http://localhost:5220/");
        }

        // Método para enviar el JSON de actualización de RTU a la API
        public async Task<string> ActualizarRtuAsync(string jsonContenido)
        {
            var content = new StringContent(jsonContenido, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/sat/actualizar-rtu", content);
            return await response.Content.ReadAsStringAsync();
        }

        // Método para enviar el JSON de facturas a la API
        public async Task<string> ProcesarFacturasAsync(string jsonContenido)
        {
            var content = new StringContent(jsonContenido, System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/sat/procesar-facturas", content);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
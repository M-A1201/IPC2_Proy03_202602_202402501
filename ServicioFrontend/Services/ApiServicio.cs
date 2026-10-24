using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ServicioFrontend.Services
{
    public class ApiServicio
    {
        private readonly HttpClient _httpClient;

        public ApiServicio(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Método para actualizar el RTU con manejo de errores amigable
        public async Task<string> ActualizarRtuAsync(string jsonContent)
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/sat/actualizar-rtu", content);
            
            string respuestaTexto = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 400)
                {
                    return " Error de sintaxis o formato en el JSON del RTU. Verifica que las llaves {}, comillas y nombres de campos estén correctos.";
                }
                return $" Error en el servidor (Código {(int)response.StatusCode}): Revisa la estructura de los datos.";
            }

            return respuestaTexto;
        }

        // Método para procesar facturas DTE con manejo de errores amigable
        public async Task<string> ProcesarFacturasAsync(string jsonContent)
        {
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/sat/procesar-facturas", content);
            
            string respuestaTexto = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 400)
                {
                    return " Error de sintaxis o formato en el JSON de las Facturas (DTE). Asegúrate de incluir campos válidos como 'referencia', 'nitEmisor', 'nitReceptor', 'valor', 'iva' y 'total'.";
                }
                return $" Error en el servidor (Código {(int)response.StatusCode}): Revisa los tipos de datos (números, fechas, textos).";
            }

            return respuestaTexto;
        }

        // Método para consultar datos
        public async Task<string> ConsultarDatosAsync(string nit, string tipo)
        {
            var response = await _httpClient.GetAsync($"api/sat/consulta-datos?nit={nit}&tipo={tipo}");
            if (!response.IsSuccessStatusCode)
            {
                return "[]";
            }
            return await response.Content.ReadAsStringAsync();
        }

        // Método para inicializar el sistema
        public async Task<string> InicializarSistemaAsync()
        {
            var response = await _httpClient.PostAsync("api/sat/inicializar", null);
            if (!response.IsSuccessStatusCode)
            {
                return " Error al intentar inicializar el sistema.";
            }
            return await response.Content.ReadAsStringAsync();
        }

        // Método para obtener el Resumen de IVA por fecha
        public async Task<string> ObtenerResumenIvaAsync(string fecha)
        {
            var response = await _httpClient.GetAsync($"api/sat/resumen-iva?fecha={fecha}");
            if (!response.IsSuccessStatusCode)
            {
                return "[]";
            }
            return await response.Content.ReadAsStringAsync();
        }

        // Método para obtener el Resumen por Rango de Fechas
        public async Task<string> ObtenerResumenRangoAsync(string fechaInicio, string fechaFin, string modo)
        {
            var response = await _httpClient.GetAsync($"api/sat/resumen-rango?fechaInicio={fechaInicio}&fechaFin={fechaFin}&modo={modo}");
            if (!response.IsSuccessStatusCode)
            {
                return "[]";
            }
            return await response.Content.ReadAsStringAsync();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicioFrontend.Models;
using ServicioFrontend.Services;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicioFrontend.Pages
{
    public class ConsultasModel : PageModel
    {
        private readonly ApiServicio _apiServicio;

        public ConsultasModel(ApiServicio apiServicio)
        {
            _apiServicio = apiServicio;
        }

        [BindProperty]
        public string Nit { get; set; } = "";

        [BindProperty]
        public string Tipo { get; set; } = "emitidas";

        public List<FacturaConsultaDto> ListaFacturas { get; set; } = new List<FacturaConsultaDto>();
        public string MensajeError { get; set; } = "";
        public bool BusquedaRealizada { get; set; } = false;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostConsultarAsync()
        {
            BusquedaRealizada = true;

            if (string.IsNullOrWhiteSpace(Nit))
            {
                MensajeError = "Por favor, ingresa un NIT para realizar la consulta.";
                return Page();
            }

            try
            {
                string jsonRespuesta = await _apiServicio.ConsultarDatosAsync(Nit, Tipo);
                
                var opciones = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var resultado = JsonSerializer.Deserialize<List<FacturaConsultaDto>>(jsonRespuesta, opciones);
                
                if (resultado != null)
                {
                    ListaFacturas = resultado;
                }
            }
            catch
            {
                MensajeError = "Ocurrió un error al procesar los datos de la consulta.";
            }

            return Page();
        }
    }
}
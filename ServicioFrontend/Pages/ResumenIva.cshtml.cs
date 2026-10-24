using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicioFrontend.Models;
using ServicioFrontend.Services;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicioFrontend.Pages
{
    public class ResumenIvaModel : PageModel
    {
        private readonly ApiServicio _apiServicio;

        public ResumenIvaModel(ApiServicio apiServicio)
        {
            _apiServicio = apiServicio;
        }

        [BindProperty]
        public string Fecha { get; set; } = "";

        public List<ResumenIvaDto> ListaResumen { get; set; } = new List<ResumenIvaDto>();
        public bool BusquedaRealizada { get; set; } = false;
        public string MensajeError { get; set; } = "";

        public void OnGet() { }

        public async Task<IActionResult> OnPostConsultarAsync()
        {
            BusquedaRealizada = true;

            if (string.IsNullOrWhiteSpace(Fecha))
            {
                MensajeError = "Por favor, ingresa una fecha (ej. 15/01/2025).";
                return Page();
            }

            try
            {
                string jsonRespuesta = await _apiServicio.ObtenerResumenIvaAsync(Fecha);
                
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultado = JsonSerializer.Deserialize<List<ResumenIvaDto>>(jsonRespuesta, opciones);

                if (resultado != null)
                {
                    ListaResumen = resultado;
                }
            }
            catch
            {
                MensajeError = "Ocurrió un error al procesar el resumen de IVA.";
            }

            return Page();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicioFrontend.Models;
using ServicioFrontend.Services;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServicioFrontend.Pages
{
    public class ResumenRangoModel : PageModel
    {
        private readonly ApiServicio _apiServicio;

        public ResumenRangoModel(ApiServicio apiServicio)
        {
            _apiServicio = apiServicio;
        }

        [BindProperty]
        public string FechaInicio { get; set; } = "";

        [BindProperty]
        public string FechaFin { get; set; } = "";

        [BindProperty]
        public string Modo { get; set; } = "total";

        public List<ResumenRangoDto> ListaRango { get; set; } = new List<ResumenRangoDto>();
        public bool BusquedaRealizada { get; set; } = false;
        public string MensajeError { get; set; } = "";

        public void OnGet() { }

        public async Task<IActionResult> OnPostConsultarAsync()
        {
            BusquedaRealizada = true;

            if (string.IsNullOrWhiteSpace(FechaInicio) || string.IsNullOrWhiteSpace(FechaFin))
            {
                MensajeError = "Por favor, ingresa tanto la fecha de inicio como la de fin.";
                return Page();
            }

            try
            {
                string jsonRespuesta = await _apiServicio.ObtenerResumenRangoAsync(FechaInicio, FechaFin, Modo);
                
                var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultado = JsonSerializer.Deserialize<List<ResumenRangoDto>>(jsonRespuesta, opciones);

                if (resultado != null)
                {
                    ListaRango = resultado;
                }
            }
            catch
            {
                MensajeError = "Ocurrió un error al procesar el resumen por rango de fechas.";
            }

            return Page();
        }
    }
}
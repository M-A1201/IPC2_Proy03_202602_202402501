using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicioFrontend.Services;
using System.Threading.Tasks;

namespace ServicioFrontend.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApiServicio _apiServicio;

        public IndexModel(ApiServicio apiServicio)
        {
            _apiServicio = apiServicio;
        }

        [BindProperty]
        public string JsonInput { get; set; } = "";

        [BindProperty]
        public string ResultadoApi { get; set; } = "";

        public void OnGet()
        {
        }

        // Acción cuando el usuario le da clic al botón de actualizar RTU
        public async Task<IActionResult> OnPostActualizarRtuAsync()
        {
            if (string.IsNullOrWhiteSpace(JsonInput))
            {
                ResultadoApi = "Por favor ingresa un JSON válido.";
                return Page();
            }

            ResultadoApi = await _apiServicio.ActualizarRtuAsync(JsonInput);
            return Page();
        }

        // Acción cuando el usuario le da clic al botón de procesar facturas
        public async Task<IActionResult> OnPostProcesarFacturasAsync()
        {
            if (string.IsNullOrWhiteSpace(JsonInput))
            {
                ResultadoApi = "Por favor ingresa un JSON válido.";
                return Page();
            }

            ResultadoApi = await _apiServicio.ProcesarFacturasAsync(JsonInput);
            return Page();
        }
    }
}
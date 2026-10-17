using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicioFrontend.Services;
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

        public string ResultadoConsulta { get; set; } = "";

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostConsultarAsync()
        {
            if (string.IsNullOrWhiteSpace(Nit))
            {
                ResultadoConsulta = "Por favor, ingresa un NIT para consultar.";
                return Page();
            }

            ResultadoConsulta = await _apiServicio.ConsultarDatosAsync(Nit, Tipo);
            return Page();
        }
    }
}
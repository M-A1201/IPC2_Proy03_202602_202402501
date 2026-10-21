using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServicioFrontend.Services;
using System.IO;
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
        public string InputRtu { get; set; } = "";

        [BindProperty]
        public string InputDte { get; set; } = "";

        public string ResultadoOperacion { get; set; } = "";

        public void OnGet() { }

        public async Task<IActionResult> OnPostActualizarRtuAsync(IFormFile? archivoRtu)
        {
            string contenido = InputRtu;

            if (archivoRtu != null && archivoRtu.Length > 0)
            {
                using var reader = new StreamReader(archivoRtu.OpenReadStream());
                contenido = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(contenido))
            {
                ResultadoOperacion = "Debes seleccionar un archivo o ingresar datos en el cuadro de texto.";
                return Page();
            }

            ResultadoOperacion = await _apiServicio.ActualizarRtuAsync(contenido);
            return Page();
        }

        public async Task<IActionResult> OnPostProcesarDteAsync(IFormFile? archivoDte)
        {
            string contenido = InputDte;

            if (archivoDte != null && archivoDte.Length > 0)
            {
                using var reader = new StreamReader(archivoDte.OpenReadStream());
                contenido = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(contenido))
            {
                ResultadoOperacion = "Debes seleccionar un archivo o ingresar datos en el cuadro de texto.";
                return Page();
            }

            ResultadoOperacion = await _apiServicio.ProcesarFacturasAsync(contenido);
            return Page();
        }

        public async Task<IActionResult> OnPostInicializarAsync()
        {
            ResultadoOperacion = await _apiServicio.InicializarSistemaAsync();
            return Page();
        }
    }
}
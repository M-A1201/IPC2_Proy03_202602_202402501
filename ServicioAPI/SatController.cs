using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class SatController : ControllerBase
{
    // Instancias estáticas o de servicio para mantener los datos en memoria mientras corre la API
    private static RegistroRTU rtuGlobal = new RegistroRTU();
    private static ProcesadorDTE procesadorDteGlobal = new ProcesadorDTE(rtuGlobal);

    // Endpoint POST para actualizar el RTU
    [HttpPost("actualizar-rtu")]
    public IActionResult ActualizarRtu([FromBody] List<Contribuyente> nuevosContribuyentes)
    {
        if (nuevosContribuyentes == null)
        {
            return BadRequest("El listado de contribuyentes no puede estar vacío.");
        }

        string resultado = rtuGlobal.ProcesarLote(nuevosContribuyentes);
        return Content(resultado, "application/json");
    }

    // Endpoint POST para procesar las solicitudes de autorización de facturas (DTE)
    [HttpPost("procesar-facturas")]
    public IActionResult ProcesarFacturas([FromBody] List<Factura> listaFacturas)
    {
        if (listaFacturas == null)
        {
            return BadRequest("El listado de facturas no puede estar vacío.");
        }

        string resultado = procesadorDteGlobal.ProcesarSolicitudes(listaFacturas);
        return Content(resultado, "application/json");
    }

    // Endpoint POST para reiniciar/inicializar el sistema
    [HttpPost("inicializar")]
    public IActionResult InicializarSistema()
    {
        rtuGlobal = new RegistroRTU();
        procesadorDteGlobal = new ProcesadorDTE(rtuGlobal);
        
        // Opcional: limpiar también los archivos locales si deseas
        if (System.IO.File.Exists("rtu_datos.json")) System.IO.File.Delete("rtu_datos.json");
        if (System.IO.File.Exists("facturas_procesadas.json")) System.IO.File.Delete("facturas_procesadas.json");

        return Ok(new { mensaje = "Sistema reiniciado al estado inicial correctamente." });
    }
}
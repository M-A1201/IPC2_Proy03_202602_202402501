using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class SatController : ControllerBase
{
    // Instancias estáticas o de servicio para mantener los datos en memoria mientras corre la API
    private static RegistroRTU rtuGlobal = new RegistroRTU();
    private static ProcesadorDTE procesadorDteGlobal = new ProcesadorDTE(rtuGlobal);


// Endpoint GET para consultar facturas emitidas o recibidas por un NIT en un rango de fechas
    /*[HttpGet("consulta-datos")]
    public IActionResult ConsultarDatos([FromQuery] string nit, [FromQuery] string tipo, [FromQuery] string fechaInicio, [FromQuery] string fechaFin)
    {
        if (string.IsNullOrEmpty(nit) || string.IsNullOrEmpty(fechaInicio) || string.IsNullOrEmpty(fechaFin))
        {
            return BadRequest("Debe proporcionar el NIT, la fecha de inicio y la fecha de fin.");
        }

        // Aquí podemos leer el archivo de facturas procesadas o buscar en memoria
        // Por ahora, retornamos una confirmación de que la ruta está activa
        return Ok(new { 
            mensaje = "Consulta recibida con éxito", 
            nitBuscado = nit, 
            tipoConsulta = tipo, 
            desde = fechaInicio, 
            hasta = fechaFin 
        });
    }*/
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

    // Método POST para inicializar/reiniciar el sistema
    [HttpPost("inicializar")]
    public IActionResult InicializarSistema()
    {
        rtuGlobal = new RegistroRTU();
        procesadorDteGlobal.LimpiarHistorial();
        
        if (System.IO.File.Exists("rtu_datos.json")) System.IO.File.Delete("rtu_datos.json");
        if (System.IO.File.Exists("facturas_procesadas.json")) System.IO.File.Delete("facturas_procesadas.json");

        return Ok(new { mensaje = "Sistema reiniciado al estado inicial correctamente." });
    }

    // Endpoint GET: /api/sat/consulta-datos (Filtrar facturas emitidas o recibidas por NIT)
    [HttpGet("consulta-datos")]
    public IActionResult ConsultarDatos([FromQuery] string nit, [FromQuery] string tipo)
    {
        if (string.IsNullOrEmpty(nit))
        {
            return BadRequest("El NIT es requerido.");
        }

        var historial = procesadorDteGlobal.ObtenerHistorial();

        if (tipo?.ToLower() == "emitidas")
        {
            var emitidas = historial.Where(f => f.NitEmisor == nit).ToList();
            return Ok(emitidas);
        }
        else if (tipo?.ToLower() == "recibidas")
        {
            var recibidas = historial.Where(f => f.NitReceptor == nit).ToList();
            return Ok(recibidas);
        }

        return Ok(historial.Where(f => f.NitEmisor == nit || f.NitReceptor == nit).ToList());
    }
}
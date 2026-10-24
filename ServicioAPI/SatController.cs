using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class SatController : ControllerBase
{

    // Endpoint GET: /api/sat/resumen-rango?fechaInicio=01/01/2025&fechaFin=31/12/2025&modo=total
    [HttpGet("resumen-rango")]
    public IActionResult ResumenRango([FromQuery] string fechaInicio, [FromQuery] string fechaFin, [FromQuery] string modo)
    {
        if (string.IsNullOrEmpty(fechaInicio) || string.IsNullOrEmpty(fechaFin))
        {
            return BadRequest("Debe proporcionar la fecha de inicio y la fecha de fin.");
        }

        var historial = procesadorDteGlobal.ObtenerHistorial();

        // Nota: Para un filtro de fechas estricto por texto en formato dd/mm/yyyy, 
        // agrupamos las facturas encontradas.
        var facturasFiltradas = historial.Where(f =>
        {
            string fFecha = ProcesadorDTE.ExtraerFechaFormato(f.Fecha);
            return !string.IsNullOrEmpty(fFecha);
        }).ToList();

        // Agrupar por fecha extraída
        var fechasUnicas = facturasFiltradas
            .Select(f => ProcesadorDTE.ExtraerFechaFormato(f.Fecha))
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        var resultado = new List<object>();

        foreach (var fecha in fechasUnicas)
        {
            var facturasDia = facturasFiltradas.Where(f => ProcesadorDTE.ExtraerFechaFormato(f.Fecha) == fecha).ToList();

            decimal montoCalculado = 0;
            if (modo?.ToLower() == "siniva")
            {
                montoCalculado = facturasDia.Sum(f => f.Valor); // Valor base sin IVA
            }
            else
            {
                montoCalculado = facturasDia.Sum(f => f.Total); // Valor total con IVA
            }

            resultado.Add(new
            {
                Fecha = fecha,
                CantidadFacturas = facturasDia.Count,
                Monto = montoCalculado
            });
        }

        return Ok(resultado);
    }




    // Instancias estáticas o de servicio para mantener los datos en memoria mientras corre la API
    private static RegistroRTU rtuGlobal = new RegistroRTU();
    private static ProcesadorDTE procesadorDteGlobal = new ProcesadorDTE(rtuGlobal);

    // Endpoint GET: /api/sat/resumen-iva?fecha=15/01/2025
    [HttpGet("resumen-iva")]
    public IActionResult ResumenIva([FromQuery] string fecha)
    {
        if (string.IsNullOrEmpty(fecha))
        {
            return BadRequest("La fecha es requerida para el resumen de IVA.");
        }

        var historial = procesadorDteGlobal.ObtenerHistorial();

        // Filtrar facturas por la fecha ingresada
        var facturasEnFecha = historial
            .Where(f => ProcesadorDTE.ExtraerFechaFormato(f.Fecha) == fecha)
            .ToList();

        // Obtener todos los NITs únicos (emisores y receptores) en esa fecha
        var nitsUnicos = facturasEnFecha.Select(f => f.NitEmisor)
            .Union(facturasEnFecha.Select(f => f.NitReceptor))
            .Distinct()
            .ToList();

        var resultado = new List<object>();

        foreach (var nit in nitsUnicos)
        {
            decimal ivaEmitido = facturasEnFecha
                .Where(f => f.NitEmisor == nit)
                .Sum(f => f.Iva);

            decimal ivaRecibido = facturasEnFecha
                .Where(f => f.NitReceptor == nit)
                .Sum(f => f.Iva);

            resultado.Add(new
            {
                Nit = nit,
                IvaEmitido = ivaEmitido,
                IvaRecibido = ivaRecibido
            });
        }

        return Ok(resultado);
    }
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
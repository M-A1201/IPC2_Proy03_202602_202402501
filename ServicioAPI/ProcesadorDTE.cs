using System;
using System.Collections.Generic;
using System.Linq;

public class ProcesadorDTE
{
    private RegistroRTU rtuRegistrado; 
    private int correlativoActual = 1;
    private string fechaUltimoCorrelativo = "";

    // Lista acumulativa para el historial de facturas aprobadas
    private static List<Factura> historialFacturasAprobadas = new List<Factura>();

    // Constructor
    public ProcesadorDTE(RegistroRTU rtu)
    {
        this.rtuRegistrado = rtu;
    }

    // Método para obtener el historial completo
    public List<Factura> ObtenerHistorial()
    {
        // Si la lista en memoria está vacía, intentamos recuperarla del archivo de respaldo si existe
        return historialFacturasAprobadas;
    }

    // Método para reiniciar el historial si se presiona "Inicializar"
    public void LimpiarHistorial()
    {
        historialFacturasAprobadas.Clear();
        correlativoActual = 1;
        fechaUltimoCorrelativo = "";
    }

    public string ProcesarSolicitudes(List<Factura> listaFacturas)
    {
        int aprobadas = 0;
        int rechazadasEmisorInexistente = 0;
        int rechazadasReceptorInexistente = 0;
        
        HashSet<string> referenciasProcesadas = new HashSet<string>();
        List<string> detalleAprobaciones = new List<string>();

        foreach (var factura in listaFacturas)
        {
            // 1. Validar si el NIT Emisor y Receptor son válidos con el Módulo 11
            if (!ValidadorNit.EsValido(factura.NitEmisor) || !ValidadorNit.EsValido(factura.NitReceptor))
            {
                continue; 
            }

            // 2. Verificar si los NITs existen en el RTU 
            bool emisorExiste = rtuRegistrado.ExisteNit(factura.NitEmisor);
            bool receptorExiste = rtuRegistrado.ExisteNit(factura.NitReceptor);

            if (!emisorExiste)
            {
                rechazadasEmisorInexistente++;
                continue;
            }

            if (!receptorExiste)
            {
                rechazadasReceptorInexistente++;
                continue;
            }

            // 3. Validar los cálculos matemáticos (IVA y Total)
            decimal ivaCalculado = Math.Round(factura.Valor * 0.12m, 2);
            decimal totalCalculado = factura.Valor + ivaCalculado;

            if (factura.Iva != ivaCalculado || factura.Total != totalCalculado)
            {
                continue;
            }

            // 4. Validar que la referencia no esté duplicada en este lote ni en el historial
            if (referenciasProcesadas.Contains(factura.Referencia) || 
                historialFacturasAprobadas.Any(f => f.Referencia == factura.Referencia))
            {
                continue;
            }
            referenciasProcesadas.Add(factura.Referencia);

            // 5. Generar el número de autorización (yyyymmdd########)
            string fechaStr = DateTime.Now.ToString("yyyyMMdd");
            
            if (fechaStr != fechaUltimoCorrelativo)
            {
                fechaUltimoCorrelativo = fechaStr;
                correlativoActual = 1;
            }

            string codigoAutorizacion = fechaUltimoCorrelativo + correlativoActual.ToString("D8");
            correlativoActual++;
            aprobadas++;

            // Guardar en el historial en memoria
            historialFacturasAprobadas.Add(factura);

            // Guardar en el detalle de la respuesta
            detalleAprobaciones.Add($"    {{\"referencia\": \"{factura.Referencia}\", \"codigoAprobacion\": {codigoAutorizacion}}}");
        }

        string detallesJson = string.Join(",\n", detalleAprobaciones);

        string jsonResultado = $@"{{
  ""resultadoAutorizaciones"": {{
    ""cantidadAutorizacionesAprobadas"": {aprobadas},
    ""cantidadAutorizacionesRechazadasPorNitEmisorInexistente"": {rechazadasEmisorInexistente},
    ""cantidadAutorizacionesRechazadasPorNitReceptorInexistente"": {rechazadasReceptorInexistente},
    ""detalleAprobaciones"": [
{detallesJson}
    ]
  }}
}}";

        try
        {
            System.IO.File.WriteAllText("facturas_procesadas.json", jsonResultado);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al guardar el archivo de facturas: " + ex.Message);
        }

        return jsonResultado;
    }
}
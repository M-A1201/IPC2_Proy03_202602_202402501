using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        return historialFacturasAprobadas;
    }

    // Método para reiniciar el historial si se presiona "Inicializar"
    public void LimpiarHistorial()
    {
        historialFacturasAprobadas.Clear();
        correlativoActual = 1;
        fechaUltimoCorrelativo = "";
    }

    // Método que utiliza Expresiones Regulares (Regex) para extraer la fecha exacta (dd/mm/yyyy)
    public static string ExtraerFechaFormato(string tiempoTexto)
    {
        if (string.IsNullOrEmpty(tiempoTexto)) return "";
        Match match = Regex.Match(tiempoTexto, @"\b(\d{2}/\d{2}/\d{4})\b");
        return match.Success ? match.Value : tiempoTexto;
    }

    public string ProcesarSolicitudes(List<Factura> listaFacturas)
    {
        int aprobadas = 0;
        int errNitEmisorInvalido = 0;
        int errNitReceptorInvalido = 0;
        int errEmisorInexistente = 0;
        int errReceptorInexistente = 0;
        int errIvaMalCalculado = 0;
        int errTotalMalCalculado = 0;
        int errReferenciaDuplicada = 0;
        
        HashSet<string> referenciasProcesadasEnLote = new HashSet<string>();
        List<string> detalleAprobaciones = new List<string>();

        foreach (var factura in listaFacturas)
        {
            // 1. Validar si el NIT Emisor es válido con el Módulo 11
            if (!ValidadorNit.EsValido(factura.NitEmisor))
            {
                errNitEmisorInvalido++;
                continue; 
            }

            // 2. Validar si el NIT Receptor es válido con el Módulo 11
            if (!ValidadorNit.EsValido(factura.NitReceptor))
            {
                errNitReceptorInvalido++;
                continue; 
            }

            // 3. Verificar si los NITs existen en el RTU 
            bool emisorExiste = rtuRegistrado.ExisteNit(factura.NitEmisor);
            bool receptorExiste = rtuRegistrado.ExisteNit(factura.NitReceptor);

            if (!emisorExiste)
            {
                errEmisorInexistente++;
                continue;
            }

            if (!receptorExiste)
            {
                errReceptorInexistente++;
                continue;
            }

            // 4. Validar los cálculos matemáticos (IVA y Total)
            decimal ivaCalculado = Math.Round(factura.Valor * 0.12m, 2);
            decimal totalCalculado = factura.Valor + ivaCalculado;

            if (factura.Iva != ivaCalculado)
            {
                errIvaMalCalculado++;
                continue;
            }

            if (factura.Total != totalCalculado)
            {
                errTotalMalCalculado++;
                continue;
            }

            // 5. Validar que la referencia no esté duplicada en este lote ni en el historial
            if (referenciasProcesadasEnLote.Contains(factura.Referencia) || 
                historialFacturasAprobadas.Any(f => f.Referencia == factura.Referencia))
            {
                errReferenciaDuplicada++;
                continue;
            }
            referenciasProcesadasEnLote.Add(factura.Referencia);

            // 6. Generar el número de autorización (yyyymmdd########)
            string fechaStr = DateTime.Now.ToString("yyyyMMdd");
            
            if (fechaStr != fechaUltimoCorrelativo)
            {
                fechaUltimoCorrelativo = fechaStr;
                correlativoActual = 1;
            }

            string codigoAutorizacion = fechaUltimoCorrelativo + correlativoActual.ToString("D8");
            correlativoActual++;
            aprobadas++;

            // Normalizar la fecha usando Regex para extraer el formato dd/mm/yyyy
            string fechaExtraida = ExtraerFechaFormato(factura.Fecha);
            if (!string.IsNullOrEmpty(fechaExtraida))
            {
                factura.Fecha = fechaExtraida;
            }

            // Guardar en el historial en memoria
            historialFacturasAprobadas.Add(factura);

            // Guardar en el detalle de la respuesta
            detalleAprobaciones.Add($"    {{\"referencia\": \"{factura.Referencia}\", \"codigoAprobacion\": {codigoAutorizacion}}}");
        }

        string detallesJson = string.Join(",\n", detalleAprobaciones);

        // Usamos una cadena interpolada normal ($"") para evitar problemas de escape con las comillas
        string jsonResultado = 
$@"{{
  ""resultadoAutorizaciones"": {{
    ""cantidadAutorizacionesAprobadas"": {aprobadas},
    ""cantidadAutorizacionesRechazadasPorNitEmisorInexistente"": {errEmisorInexistente},
    ""cantidadAutorizacionesRechazadasPorNitReceptorInexistente"": {errReceptorInexistente},
    ""detalleAprobaciones"": [
{detallesJson}
    ]
  }}
}}";

        try
        {
            File.WriteAllText("facturas_procesadas.json", jsonResultado);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al guardar el archivo de facturas: " + ex.Message);
        }

        return jsonResultado;
    }
}
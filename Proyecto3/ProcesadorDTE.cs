using System;
using System.Collections.Generic;

public class ProcesadorDTE
{
    private RegistroRTU rtuRegistrado; 
    private int correlativoActual = 1;
    private string fechaUltimoCorrelativo = "";

    // Constructor
    public ProcesadorDTE(RegistroRTU rtu)
    {
        this.rtuRegistrado = rtu;
    }

    public string ProcesarSolicitudes(List<Factura> listaFacturas)
    {
        int aprobadas = 0;
        int rechazadasEmisorInexistente = 0;
        int rechazadasReceptorInexistente = 0;
        
        // Lista para controlar referencias únicas dentro del lote y evitar duplicados
        HashSet<string> referenciasProcesadas = new HashSet<string>();
        
        // Lista para guardar el detalle de las aprobaciones que exige el JSON de respuesta
        List<string> detalleAprobaciones = new List<string>();

        foreach (var factura in listaFacturas)
        {
            // PASO 1: Validar si el NIT Emisor y Receptor son válidos con el Módulo 11
            if (!ValidadorNit.EsValido(factura.NitEmisor) || !ValidadorNit.EsValido(factura.NitReceptor))
            {
                // Aquí puedes contar los NITs inválidos si deseas
                continue; 
            }

            // PASO 2: Verificar si los NITs existen en el RTU 
            // (Asumiremos que RegistroRTU tiene un método ExisteNit(string nit))
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

            // PASO 3: Validar los cálculos matemáticos (IVA y Total)
            // REDONDEAR(VALOR * 0.12, 2)
            decimal ivaCalculado = Math.Round(factura.Valor * 0.12m, 2);
            decimal totalCalculado = factura.Valor + ivaCalculado;

            if (factura.Iva != ivaCalculado || factura.Total != totalCalculado)
            {
                // Error de cálculo en IVA o Total
                continue;
            }

            // PASO 4: Validar que la referencia no esté duplicada
            if (referenciasProcesadas.Contains(factura.Referencia))
            {
                // Referencia duplicada
                continue;
            }
            referenciasProcesadas.Add(factura.Referencia);

            // PASO 5: Generar el número de autorización (yyyymmdd########)
            // Extraemos la fecha de la factura o usamos la fecha actual del sistema para el correlativo
            string fechaStr = DateTime.Now.ToString("yyyyMMdd"); // Formato yyyymmdd para el inicio del código
            
            if (fechaStr != fechaUltimoCorrelativo)
            {
                fechaUltimoCorrelativo = fechaStr;
                correlativoActual = 1; // Se reinicia a 1 cada día como pide el enunciado
            }

            // Formatear el correlativo con 8 dígitos precedidos de ceros (ej. 00000001)
            string codigoAutorizacion = fechaUltimoCorrelativo + correlativoActual.ToString("D8");
            correlativoActual++;
            aprobadas++;

            // Guardamos el detalle para la respuesta JSON
            detalleAprobaciones.Add($"    {{\"referencia\": \"{factura.Referencia}\", \"codigoAprobacion\": {codigoAutorizacion}}}");
        }

        string detallesJson = string.Join(",\n", detalleAprobaciones);

        // PASO 6: Retornar el JSON con el formato exacto requerido por el enunciado
        return $@"{{
  ""resultadoAutorizaciones"": {{
    ""cantidadAutorizacionesAprobadas"": {aprobadas},
    ""cantidadAutorizacionesRechazadasPorNitEmisorInexistente"": {rechazadasEmisorInexistente},
    ""cantidadAutorizacionesRechazadasPorNitReceptorInexistente"": {rechazadasReceptorInexistente},
    ""detalleAprobaciones"": [
{detallesJson}
    ]
  }}
}}";
    }
}
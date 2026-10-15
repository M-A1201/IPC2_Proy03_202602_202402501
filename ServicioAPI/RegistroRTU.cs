using System;
using System.Collections.Generic;

public class RegistroRTU
{

    //metodo 
    public bool ExisteNit(string nit)
{
    return listaContribuyentes.Exists(c => c.Nit == nit);
}

    // Lista interna para almacenar los contribuyentes (nuestro RTU en memoria)
    private List<Contribuyente> listaContribuyentes = new List<Contribuyente>();

    // Método principal para procesar la lista de contribuyentes que viene del JSON
    public string ProcesarLote(List<Contribuyente> nuevosContribuyentes)
    {
        int contNuevos = 0;
        int contActualizados = 0;
        int contInvalidos = 0;

        foreach (var item in nuevosContribuyentes)
        {
            // 1. Validar el NIT usando nuestra clase ValidorNit
            if (!ValidadorNit.EsValido(item.Nit))
            {
                contInvalidos++;
                continue; // Si no es válido, no tiene ningún efecto en el RTU
            }

            // 2. Buscar si el NIT ya existe en nuestro RTU
            var existente = listaContribuyentes.Find(c => c.Nit == item.Nit);

            if (existente == null)
            {
                // Si es válido y no existe, se crea un nuevo contribuyente
                listaContribuyentes.Add(new Contribuyente(item.Nit, item.Nombre));
                contNuevos++;
            }
            else
            {
                // Si es válido y ya existe, se actualizará el contribuyente[cite: 3]
                existente.Nombre = item.Nombre;
                contActualizados++;
            }
        }
// 3. Devolver la respuesta exactamente con la estructura requerida por el enunciado
        string jsonRespuesta = $"{{\n  \"CONTRIBUYENTES NUEVOS\": {contNuevos},\n  \"CONTRIBUYENTES ACTUALIZADOS\": {contActualizados},\n  \"NITS INVALIDOS\": {contInvalidos}\n}}";

        // === NUEVO: GUARDAR EL RTU EN UN ARCHIVO LOCAL ===
        try
        {
            // Convertimos nuestra lista de contribuyentes a texto plano (JSON básico)
            List<string> lineasContribuyentes = new List<string>();
            foreach (var c in listaContribuyentes)
            {
                lineasContribuyentes.Add($"{{\"NIT\": \"{c.Nit}\", \"nombre\": \"{c.Nombre}\"}}");
            }
            string contenidoArchivo = "[\n" + string.Join(",\n", lineasContribuyentes) + "\n]";
            
            // Guarda el archivo en la carpeta del proyecto
            System.IO.File.WriteAllText("rtu_datos.json", contenidoArchivo);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al guardar el archivo RTU: " + ex.Message);
        }

        return jsonRespuesta;
    }
}
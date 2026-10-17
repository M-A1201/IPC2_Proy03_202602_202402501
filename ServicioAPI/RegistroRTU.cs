using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public class RegistroRTU
{
    // Lista interna para almacenar los contribuyentes (nuestro RTU en memoria)
    private List<Contribuyente> listaContribuyentes = new List<Contribuyente>();

    // === CONSTRUCTOR: Carga los datos del archivo si ya existe para no perderlos en memoria ===
    public RegistroRTU()
    {
        if (File.Exists("rtu_datos.json"))
        {
            try
            {
                string jsonContenido = File.ReadAllText("rtu_datos.json");
                // Deserializamos el archivo guardado previamente a nuestra lista en memoria
                var contribuyentesGuardados = JsonSerializer.Deserialize<List<Contribuyente>>(jsonContenido, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (contribuyentesGuardados != null)
                {
                    listaContribuyentes = contribuyentesGuardados;
                }
            }
            catch (Exception)
            {
                // Si ocurre algún error leyendo el archivo, lo dejamos vacío de forma segura
            }
        }
    }

    // Método para verificar si un NIT existe
    public bool ExisteNit(string nit)
    {
        return listaContribuyentes.Exists(c => c.Nit == nit);
    }

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
                continue; 
            }

            // 2. Buscar si el NIT ya existe en nuestro RTU
            var existente = listaContribuyentes.Find(c => c.Nit == item.Nit);

            if (existente == null)
            {
                listaContribuyentes.Add(new Contribuyente(item.Nit, item.Nombre));
                contNuevos++;
            }
            else
            {
                existente.Nombre = item.Nombre;
                contActualizados++;
            }
        }

        string jsonRespuesta = $"{{\n  \"CONTRIBUYENTES NUEVOS\": {contNuevos},\n  \"CONTRIBUYENTES ACTUALIZADOS\": {contActualizados},\n  \"NITS INVALIDOS\": {contInvalidos}\n}}";

        // === GUARDAR EL RTU EN UN ARCHIVO LOCAL ===
        try
        {
            List<string> lineasContribuyentes = new List<string>();
            foreach (var c in listaContribuyentes)
            {
                lineasContribuyentes.Add($"{{\"NIT\": \"{c.Nit}\", \"nombre\": \"{c.Nombre}\"}}");
            }
            string contenidoArchivo = "[\n" + string.Join(",\n", lineasContribuyentes) + "\n]";

            File.WriteAllText("rtu_datos.json", contenidoArchivo);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al guardar el archivo RTU: " + ex.Message);
        }

        return jsonRespuesta;
    }
}
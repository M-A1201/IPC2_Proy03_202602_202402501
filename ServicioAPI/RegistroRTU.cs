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
        return $"{{\n  \"CONTRIBUYENTES NUEVOS\": {contNuevos},\n  \"CONTRIBUYENTES ACTUALIZADOS\": {contActualizados},\n  \"NITS INVALIDOS\": {contInvalidos}\n}}";
    }
}
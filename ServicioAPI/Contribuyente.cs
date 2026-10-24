using System.Text.Json.Serialization;

public class Contribuyente
{
    private string nit = "";
    private string nombre = "";

    // Constructor vacío necesario para deserialización JSON
    public Contribuyente() { }

    public Contribuyente(string nit, string nombre)
    {
        this.nit = nit;
        this.nombre = nombre;
    }

    [JsonPropertyName("NIT")]
    public string Nit 
    { 
        get { return nit; } 
        set { nit = value; } 
    }

    [JsonPropertyName("nombre")]
    public string Nombre 
    { 
        get { return nombre; } 
        set { nombre = value; } 
    }
}
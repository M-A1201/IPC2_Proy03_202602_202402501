using System;
using System.Text.Json.Serialization;

public class Factura
{
    private string fecha = "";
    private string referencia = "";
    private string nitEmisor = "";
    private string nitReceptor = "";
    private decimal valor;
    private decimal iva;
    private decimal total;

    // Constructor vacío indispensable para la deserialización JSON
    public Factura() { }

    public Factura(string fecha, string referencia, string nitEmisor, string nitReceptor, decimal valor, decimal iva, decimal total)
    {
        this.fecha = fecha;
        this.referencia = referencia;
        this.nitEmisor = nitEmisor;
        this.nitReceptor = nitReceptor;
        this.valor = valor;
        this.iva = iva;
        this.total = total;
    }

    [JsonPropertyName("TIEMPO")]
    public string Fecha 
    { 
        get { return fecha; } 
        set { fecha = value; } 
    }

    [JsonPropertyName("REFERENCIA")]
    public string Referencia 
    { 
        get { return referencia; } 
        set { referencia = value; } 
    }

    // Soporte tanto para "NIT EMISOR" como para "NIT_EMISOR"
    [JsonPropertyName("NIT EMISOR")]
    public string NitEmisorEspacio { set { nitEmisor = value; } }

    [JsonPropertyName("NIT_EMISOR")]
    public string NitEmisorGuion { set { nitEmisor = value; } }

    public string NitEmisor 
    { 
        get { return nitEmisor; } 
        set { nitEmisor = value; } 
    }

    // Soporte tanto para "NIT RECEPTOR" como para "NIT_RECEPTOR"
    [JsonPropertyName("NIT RECEPTOR")]
    public string NitReceptorEspacio { set { nitReceptor = value; } }

    [JsonPropertyName("NIT_RECEPTOR")]
    public string NitReceptorGuion { set { nitReceptor = value; } }

    public string NitReceptor 
    { 
        get { return nitReceptor; } 
        set { nitReceptor = value; } 
    }

    [JsonPropertyName("VALOR")]
    public decimal Valor 
    { 
        get { return valor; } 
        set { valor = value; } 
    }

    [JsonPropertyName("IVA")]
    public decimal Iva 
    { 
        get { return iva; } 
        set { iva = value; } 
    }

    [JsonPropertyName("TOTAL")]
    public decimal Total 
    { 
        get { return total; } 
        set { total = value; } 
    }
}
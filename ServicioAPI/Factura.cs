using System;
using System.Text.Json.Serialization;

public class Factura
{
    private string fecha;
    private string referencia;
    private string nitEmisor;
    private string nitReceptor;
    private decimal valor;
    private decimal iva;
    private decimal total;

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

    [JsonPropertyName("NIT EMISOR")]
    public string NitEmisor 
    { 
        get { return nitEmisor; } 
        set { nitEmisor = value; } 
    }

    [JsonPropertyName("NIT RECEPTOR")]
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
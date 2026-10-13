using System;

public class Factura
{
    // Atributos privados
    private string fecha;
    private string referencia;
    private string nitEmisor;
    private string nitReceptor;
    private decimal valor;
    private decimal iva;
    private decimal total;

    // Constructor (opcional para inicializar los datos)
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

    // Propiedades públicas (Getters y Setters) que resuelven el error
    public string Fecha 
    { 
        get { return fecha; } 
        set { fecha = value; } 
    }

    public string Referencia 
    { 
        get { return referencia; } 
        set { referencia = value; } 
    }

    public string NitEmisor 
    { 
        get { return nitEmisor; } 
        set { nitEmisor = value; } 
    }

    public string NitReceptor 
    { 
        get { return nitReceptor; } 
        set { nitReceptor = value; } 
    }

    public decimal Valor 
    { 
        get { return valor; } 
        set { valor = value; } 
    }

    public decimal Iva 
    { 
        get { return iva; } 
        set { iva = value; } 
    }

    public decimal Total 
    { 
        get { return total; } 
        set { total = value; } 
    }
}
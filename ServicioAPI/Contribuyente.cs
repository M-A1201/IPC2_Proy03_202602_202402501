using System;

public class Contribuyente
{
    private string nit;
    private string nombre;

    public Contribuyente(string nit, string nombre)
    {
        this.nit = nit;
        this.nombre = nombre;
    }

    public string Nit 
    { 
        get { return nit; } 
        set { nit = value; } 
    }

    public string Nombre 
    { 
        get { return nombre; } 
        set { nombre = value; } 
    }
}
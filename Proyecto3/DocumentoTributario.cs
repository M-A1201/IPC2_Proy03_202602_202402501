using System;

public class DocumentoTributario : Factura
{
    private string codigoAutorizacion;

    public DocumentoTributario(string fecha, string referencia, string nitEmisor, string nitReceptor, decimal valor, decimal iva, decimal total, string codigoAutorizacion) 
        : base(fecha, referencia, nitEmisor, nitReceptor, valor, iva, total)
    {
        this.codigoAutorizacion = codigoAutorizacion;
    }

    public string CodigoAutorizacion
    {
        get { return codigoAutorizacion; }
        set { codigoAutorizacion = value; }
    }
}
using System;

public class Factura
{
    private string fecha; // Lo manejamos como string para capturar tal cual viene del JSON (ej. "Guatemala, 15/01/2025 15:25 hrs.") o DateTime si prefieres parsearlo
    private string referencia;
    private string nitEmisor;
    private string nitReceptor;
    private decimal valor;
    private decimal iva;
    private decimal total;

    // Constructor y propiedades aquí...
}
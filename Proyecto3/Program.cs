using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== INICIANDO PRUEBAS DEL SISTEMA SAT (DTE & RTU) ===");

        // 1. Instanciamos nuestro Registro RTU
        RegistroRTU rtu = new RegistroRTU();

        // Simulamos la llegada del JSON de actualización de RTU con contribuyentes de prueba
        // (Usamos NITs válidos según el algoritmo de Módulo 11)
        List<Contribuyente> nuevosContribuyentes = new List<Contribuyente>
        {
            new Contribuyente("7378106", "Empresa ABC, S.A."),
            new Contribuyente("8338817", "Comercial XYZ, S.A."),
            new Contribuyente("123456K", "Contribuyente Falso S.A.") // Este tiene NIT inválido para probar el filtro
        };

        Console.WriteLine("\n--- Procesando actualización de RTU ---");
        string respuestaRtu = rtu.ProcesarLote(nuevosContribuyentes);
        Console.WriteLine(respuestaRtu);

        // 2. Instanciamos el Procesador de DTEs pasándole el RTU ya cargado
        ProcesadorDTE procesadorDte = new ProcesadorDTE(rtu);

        // Simulamos la llegada de facturas a través de un JSON de solicitudes
        List<Factura> listaFacturas = new List<Factura>
        {
            new Factura("Guatemala, 15/01/2025 15:25 hrs.", "A1990", "7378106", "8338817", 100.00m, 12.00m, 112.00m),
            new Factura("Guatemala, 15/01/2025 15:25 hrs.", "A1991", "7378106", "8338817", 200.00m, 24.00m, 224.00m)
        };

        Console.WriteLine("\n--- Procesando solicitudes de autorización de Facturas ---");
        string respuestaAutorizaciones = procesadorDte.ProcesarSolicitudes(listaFacturas);
        Console.WriteLine(respuestaAutorizaciones);

        Console.WriteLine("\n=== FIN DE LA PRUEBA ===");
        Console.ReadKey();
    }
}
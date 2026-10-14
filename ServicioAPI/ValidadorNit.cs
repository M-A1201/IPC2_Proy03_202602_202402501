using System;

public class ValidadorNit
{
    //metodo para verificar la validacion del nit
    public static bool EsValido(string nit)
    {
        if (string.IsNullOrWhiteSpace(nit))
        {
            return false;
        }

        nit = nit.Trim().ToUpper();

        if (nit.Length < 2)
        {
            return false;
        }

        // El último carácter a la derecha es el dígito verificador (V)
        char digitoVerificadorIngresado = nit[nit.Length - 1];

        // El cuerpo del NIT es todo lo demás (excluyendo la posición 1 que es el dígito verificador)
        string cuerpo = nit.Substring(0, nit.Length - 1);

        int suma = 0;
        int posicion = 2; // Empezamos desde la posición 2, ya que la posición 1 es el dígito verificador

        // Recorremos el cuerpo de derecha a izquierda
        for (int i = cuerpo.Length - 1; i >= 0; i--)
        {
            char caracter = cuerpo[i];

            // Validar que cada carácter del cuerpo sea un número entre 0 y 9
            if (!char.IsDigit(caracter))
            {
                return false;
            }

            int numero = int.Parse(caracter.ToString());

            // 1. Multiplicar cada carácter por su posición respectiva
            suma += numero * posicion;
            posicion++;
        }

        // 2 y 3. Obtener el módulo 11 de la sumatoria
        int modulo = suma % 11;

        // 4. A 11 réstale el resultado obtenido en el punto 3
        int resta = 11 - modulo;

        // 5. Calcule el módulo 11 del resultado obtenido en el punto No. 4
        int resultadoFinal = resta % 11;

        string digitoCalculado;

        // Si este resultado es 10, el dígito verificador deberá ser K
        if (resultadoFinal == 10)
        {
            digitoCalculado = "K";
        }
        else
        {
            digitoCalculado = resultadoFinal.ToString();
        }

        // Comparamos el dígito calculado con el dígito ingresado por el usuario
        return digitoCalculado == digitoVerificadorIngresado.ToString();
    }
}
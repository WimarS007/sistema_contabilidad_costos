/// <summary>
/// Utilidad para imprimir tablas bien formadas en la consola,
/// con bordes Unicode, cabeceras resaltadas y columnas alineadas.
/// </summary>
static class TablaConsola
{
    // ── Caracteres de borde ────────────────────────────────────────────────
    const char TOP_L    = '╔'; const char TOP_M    = '╦'; const char TOP_R    = '╗';
    const char MID_L    = '╠'; const char MID_M    = '╬'; const char MID_R    = '╣';
    const char BOT_L    = '╚'; const char BOT_M    = '╩'; const char BOT_R    = '╝';
    const char SEP_L    = '╟'; const char SEP_M    = '╫'; const char SEP_R    = '╢';
    const char H_DOBLE  = '═'; const char H_SIMPLE = '─';
    const char V_DOBLE  = '║'; const char V_SIMPLE = '│';

    /// <summary>
    /// Imprime una tabla en consola.
    /// </summary>
    /// <param name="cabeceras">Nombres de columna.</param>
    /// <param name="filas">Filas de datos (string[]).</param>
    /// <param name="alinDer">Índices de columnas alineadas a la derecha (ej: montos).</param>
    /// <param name="indent">Espacios de sangría izquierda.</param>
    public static void Imprimir(
        string[]   cabeceras,
        List<string[]> filas,
        int[]?     alinDer = null,
        string     indent  = "  ")
    {
        int cols = cabeceras.Length;
        int[] anchos = new int[cols];

        // Calcular ancho mínimo de cada columna
        for (int i = 0; i < cols; i++)
            anchos[i] = cabeceras[i].Length;

        foreach (var fila in filas)
            for (int i = 0; i < cols; i++)
                if (i < fila.Length)
                    anchos[i] = Math.Max(anchos[i], fila[i].Length);

        // Añadir padding interno
        for (int i = 0; i < cols; i++)
            anchos[i] += 2;

        // Conjuntos de alineación derecha
        HashSet<int> derecha = alinDer != null
            ? new HashSet<int>(alinDer)
            : new HashSet<int>();

        // ── Línea superior doble ─────────────────────────────────────────
        Console.Write(indent + TOP_L);
        for (int i = 0; i < cols; i++)
        {
            Console.Write(new string(H_DOBLE, anchos[i]));
            Console.Write(i < cols - 1 ? TOP_M : TOP_R);
        }
        Console.WriteLine();

        // ── Cabeceras ────────────────────────────────────────────────────
        Console.Write(indent + V_DOBLE);
        for (int i = 0; i < cols; i++)
        {
            string celda = Centrar(cabeceras[i], anchos[i] - 2);
            Console.Write($" {celda} {V_DOBLE}");
        }
        Console.WriteLine();

        // ── Separador doble bajo cabecera ────────────────────────────────
        Console.Write(indent + MID_L);
        for (int i = 0; i < cols; i++)
        {
            Console.Write(new string(H_DOBLE, anchos[i]));
            Console.Write(i < cols - 1 ? MID_M : MID_R);
        }
        Console.WriteLine();

        // ── Filas de datos ───────────────────────────────────────────────
        for (int r = 0; r < filas.Count; r++)
        {
            var fila = filas[r];
            Console.Write(indent + V_SIMPLE);
            for (int i = 0; i < cols; i++)
            {
                string valor = (i < fila.Length) ? fila[i] : "";
                string celda = derecha.Contains(i)
                    ? valor.PadLeft(anchos[i] - 2)
                    : valor.PadRight(anchos[i] - 2);
                Console.Write($" {celda} {V_SIMPLE}");
            }
            Console.WriteLine();

            // Separador simple entre filas (no después de la última)
            if (r < filas.Count - 1)
            {
                Console.Write(indent + SEP_L);
                for (int i = 0; i < cols; i++)
                {
                    Console.Write(new string(H_SIMPLE, anchos[i]));
                    Console.Write(i < cols - 1 ? SEP_M : SEP_R);
                }
                Console.WriteLine();
            }
        }

        // ── Línea inferior doble ─────────────────────────────────────────
        Console.Write(indent + BOT_L);
        for (int i = 0; i < cols; i++)
        {
            Console.Write(new string(H_DOBLE, anchos[i]));
            Console.Write(i < cols - 1 ? BOT_M : BOT_R);
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Imprime una tabla de resumen de dos columnas (concepto | monto).
    /// La última fila se trata como total y se separa con línea doble.
    /// </summary>
    public static void ImprimirResumen(
        string titulo,
        List<(string Concepto, string Monto)> filas,
        string totalConcepto,
        string totalMonto,
        string indent = "  ")
    {
        // Calcular anchos
        int anchoConc = titulo.Length;
        int anchoMont = "Monto".Length;
        foreach (var (c, m) in filas)
        {
            anchoConc = Math.Max(anchoConc, c.Length);
            anchoMont = Math.Max(anchoMont, m.Length);
        }
        anchoConc = Math.Max(anchoConc, totalConcepto.Length) + 2;
        anchoMont = Math.Max(anchoMont, totalMonto.Length)    + 2;

        // Cabecera
        Console.Write(indent + TOP_L);
        Console.Write(new string(H_DOBLE, anchoConc));
        Console.Write(TOP_M);
        Console.Write(new string(H_DOBLE, anchoMont));
        Console.WriteLine(TOP_R);

        Console.Write(indent + V_DOBLE);
        Console.Write($" {Centrar(titulo, anchoConc - 2)} ");
        Console.Write(V_DOBLE);
        Console.Write($" {"Monto".PadLeft(anchoMont - 2)} ");
        Console.WriteLine(V_DOBLE);

        Console.Write(indent + MID_L);
        Console.Write(new string(H_DOBLE, anchoConc));
        Console.Write(MID_M);
        Console.Write(new string(H_DOBLE, anchoMont));
        Console.WriteLine(MID_R);

        // Filas
        for (int r = 0; r < filas.Count; r++)
        {
            var (c, m) = filas[r];
            Console.Write(indent + V_SIMPLE);
            Console.Write($" {c.PadRight(anchoConc - 2)} ");
            Console.Write(V_SIMPLE);
            Console.Write($" {m.PadLeft(anchoMont - 2)} ");
            Console.WriteLine(V_SIMPLE);

            if (r < filas.Count - 1)
            {
                Console.Write(indent + SEP_L);
                Console.Write(new string(H_SIMPLE, anchoConc));
                Console.Write(SEP_M);
                Console.Write(new string(H_SIMPLE, anchoMont));
                Console.WriteLine(SEP_R);
            }
        }

        // Separador doble antes del total
        Console.Write(indent + MID_L);
        Console.Write(new string(H_DOBLE, anchoConc));
        Console.Write(MID_M);
        Console.Write(new string(H_DOBLE, anchoMont));
        Console.WriteLine(MID_R);

        // Fila total
        Console.Write(indent + V_DOBLE);
        Console.Write($" {totalConcepto.PadRight(anchoConc - 2)} ");
        Console.Write(V_DOBLE);
        Console.Write($" {totalMonto.PadLeft(anchoMont - 2)} ");
        Console.WriteLine(V_DOBLE);

        // Cierre
        Console.Write(indent + BOT_L);
        Console.Write(new string(H_DOBLE, anchoConc));
        Console.Write(BOT_M);
        Console.Write(new string(H_DOBLE, anchoMont));
        Console.WriteLine(BOT_R);
    }

    // ── Helpers privados ─────────────────────────────────────────────────
    static string Centrar(string texto, int ancho)
    {
        if (texto.Length >= ancho) return texto.Substring(0, ancho);
        int izq = (ancho - texto.Length) / 2;
        int der = ancho - texto.Length - izq;
        return new string(' ', izq) + texto + new string(' ', der);
    }
}

/// <summary>
/// Módulo PEPS — Primero en Entrar, Primero en Salir (FIFO).
/// Mantiene una cola de lotes ordenada por fecha de entrada.
/// Al registrar una salida, consume primero los lotes más antiguos.
/// Genera el kardex completo con desglose por lote consumido.
/// </summary>
class ModuloPEPS
{
    // ── Fila interna del kardex ────────────────────────────────────────────

    private class FilaKardex
    {
        public string Fecha       = "";
        public string Descripcion = "";
        public string EntradaUnd  = "";
        public string SalidaUnd   = "";
        public string SaldoUnd    = "";
        public string CostoUnit   = "";
        public string ValEntrada  = "";
        public string ValSalida   = "";
        public string ValSaldo    = "";
    }

    // ── Estado interno ─────────────────────────────────────────────────────

    private readonly LinkedList<LotePEPS> lotes   = new();
    private readonly List<FilaKardex>     kardex  = new();

    private decimal saldoCantidad   = 0;
    private decimal saldoValor      = 0;
    private decimal acumEntradas    = 0;   // total valor de entradas
    private decimal acumSalidas     = 0;   // total valor de salidas (costo de ventas)

    // ── Propiedades públicas ───────────────────────────────────────────────

    public bool    TieneMovimientos  => kardex.Count > 0;
    public decimal TotalEntradas     => acumEntradas;
    public decimal CostoVentas       => acumSalidas;      // material consumido valorado PEPS
    public decimal InventarioFinal   => saldoValor;
    public decimal UnidadesDisponibles => saldoCantidad;

    // ── CRUD ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Registra una entrada (inventario inicial o compra).
    /// Crea un nuevo lote al final de la cola.
    /// </summary>
    public void RegistrarEntrada(string fecha, string descripcion,
                                 decimal cantidad, decimal costoUnitario)
    {
        if (cantidad <= 0 || costoUnitario < 0)
        {
            Console.WriteLine("  ⚠  Cantidad debe ser > 0 y costo unitario ≥ 0.");
            return;
        }

        decimal valor = cantidad * costoUnitario;

        lotes.AddLast(new LotePEPS(cantidad, costoUnitario));

        saldoCantidad += cantidad;
        saldoValor    += valor;
        acumEntradas  += valor;

        kardex.Add(new FilaKardex
        {
            Fecha       = fecha,
            Descripcion = descripcion,
            EntradaUnd  = cantidad.ToString("N2"),
            SaldoUnd    = saldoCantidad.ToString("N2"),
            CostoUnit   = $"C$ {costoUnitario:N2}",
            ValEntrada  = $"C$ {valor:N2}",
            ValSaldo    = $"C$ {saldoValor:N2}"
        });

        Console.WriteLine($"\n  ✔  Entrada: {cantidad:N2} u × C$ {costoUnitario:N2} = C$ {valor:N2}  " +
                          $"| Saldo: {saldoCantidad:N2} u");
    }

    /// <summary>
    /// Registra una salida (venta / consumo).
    /// Consume lotes en orden PEPS; si la salida abarca varios lotes
    /// se genera una fila de kardex por cada lote consumido.
    /// </summary>
    public bool RegistrarSalida(string fecha, string descripcion, decimal cantidad)
    {
        if (cantidad <= 0)
        {
            Console.WriteLine("  ⚠  La cantidad de salida debe ser mayor a cero.");
            return false;
        }

        if (cantidad > saldoCantidad)
        {
            Console.WriteLine($"  ⚠  Stock insuficiente.  Disponible: {saldoCantidad:N2} u — " +
                              $"Solicitado: {cantidad:N2} u.");
            return false;
        }

        decimal pendiente   = cantidad;
        bool    primeraFila = true;
        decimal totalSalida = 0;

        while (pendiente > 0 && lotes.First != null)
        {
            LotePEPS lote   = lotes.First.Value;
            decimal  tomado = Math.Min(pendiente, lote.Cantidad);
            decimal  valor  = tomado * lote.CostoUnitario;

            lote.Cantidad    -= tomado;
            pendiente        -= tomado;
            saldoCantidad    -= tomado;
            saldoValor       -= valor;
            acumSalidas      += valor;
            totalSalida      += valor;

            if (lote.Cantidad == 0)
                lotes.RemoveFirst();

            kardex.Add(new FilaKardex
            {
                Fecha       = primeraFila ? fecha : "",
                Descripcion = primeraFila ? descripcion : "  ↳ continuación",
                SalidaUnd   = tomado.ToString("N2"),
                SaldoUnd    = saldoCantidad.ToString("N2"),
                CostoUnit   = $"C$ {lote.CostoUnitario:N2}",
                ValSalida   = $"C$ {valor:N2}",
                ValSaldo    = $"C$ {saldoValor:N2}"
            });

            primeraFila = false;
        }

        Console.WriteLine($"\n  ✔  Salida: {cantidad:N2} u — Costo PEPS: C$ {totalSalida:N2}  " +
                          $"| Saldo: {saldoCantidad:N2} u");
        return true;
    }

    /// <summary>
    /// Elimina todos los movimientos (reinicia el módulo).
    /// </summary>
    public void LimpiarMovimientos()
    {
        lotes.Clear();
        kardex.Clear();
        saldoCantidad = saldoValor = acumEntradas = acumSalidas = 0;
        Console.WriteLine("\n  ✔  Kardex PEPS reiniciado.");
    }

    // ── Reporte ────────────────────────────────────────────────────────────

    /// <summary>
    /// Imprime el kardex PEPS completo y el resumen de valuación.
    /// </summary>
    public void ImprimirKardex()
    {
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║       MÓDULO PEPS (FIFO) — KARDEX DE INVENTARIO              ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");

        if (kardex.Count == 0)
        {
            Console.WriteLine("\n  (Sin movimientos registrados)\n");
            return;
        }

        // ── Tabla kardex ─────────────────────────────────────────────────
        var filas = new List<string[]>();
        foreach (FilaKardex f in kardex)
        {
            filas.Add(new[]
            {
                f.Fecha,
                f.Descripcion,
                f.EntradaUnd,
                f.SalidaUnd,
                f.SaldoUnd,
                f.CostoUnit,
                f.ValEntrada,
                f.ValSalida,
                f.ValSaldo
            });
        }

        TablaConsola.Imprimir(
            new[] { "Fecha", "Descripción", "Entradas", "Salidas", "Saldo", "C. Unit.", "Val. Entrada", "Val. Salida", "Val. Saldo" },
            filas,
            alinDer: new[] { 2, 3, 4, 6, 7, 8 }
        );

        // ── Resumen PEPS ─────────────────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("  ▸ RESUMEN DE VALUACIÓN PEPS\n");

        var resumen = new List<(string, string)>
        {
            ("Total Entradas (valor compras)",   $"C$ {acumEntradas,14:N2}"),
            ("Costo de Ventas PEPS",             $"C$ {acumSalidas,14:N2}"),
            ("Inventario Final",                 $"C$ {saldoValor,14:N2}"),
            ("Verificación (Entradas - Ventas)", $"C$ {(acumEntradas - acumSalidas),14:N2}")
        };

        TablaConsola.ImprimirResumen(
            titulo:        "Concepto PEPS",
            filas:         resumen,
            totalConcepto: "Unidades disponibles",
            totalMonto:    $"{saldoCantidad,14:N2} u"
        );

        // ── Lotes actuales en cola ────────────────────────────────────────
        if (lotes.Count > 0)
        {
            Console.WriteLine("\n  ▸ LOTES EN EXISTENCIA (cola PEPS actual)\n");
            int    num   = 1;
            var    fLotes = new List<string[]>();
            foreach (LotePEPS l in lotes)
            {
                fLotes.Add(new[]
                {
                    num++.ToString(),
                    l.Cantidad.ToString("N2"),
                    $"C$ {l.CostoUnitario:N2}",
                    $"C$ {l.ValorTotal:N2}"
                });
            }
            TablaConsola.Imprimir(
                new[] { "#", "Cantidad", "Costo Unit.", "Valor Lote" },
                fLotes,
                alinDer: new[] { 1, 2, 3 }
            );
        }

        Console.WriteLine();
        Console.WriteLine("  " + new string('═', 62));
    }

    /// <summary>
    /// Imprime un mini-resumen para incrustar en el reporte final de producción.
    /// </summary>
    public void ImprimirResumenParaReporte()
    {
        if (!TieneMovimientos) return;

        Console.WriteLine("\n  ▸ VALUACIÓN PEPS VINCULADA AL REPORTE\n");

        var filas = new List<string[]>
        {
            new[] { "Costo de Ventas (materiales consumidos — PEPS)",  $"C$ {acumSalidas:N2}"   },
            new[] { "Inventario Final  (materiales en existencia)",     $"C$ {saldoValor:N2}"    },
            new[] { "Total Entradas al período",                        $"C$ {acumEntradas:N2}"  },
        };

        TablaConsola.Imprimir(
            new[] { "Dato PEPS", "Valor" },
            filas,
            alinDer: new[] { 1 }
        );

        Console.WriteLine($"\n  Nota: el Costo de Ventas PEPS (C$ {acumSalidas:N2}) representa");
        Console.WriteLine("        el costo de los materiales consumidos valorados bajo el");
        Console.WriteLine("        método PEPS. Compáralo con el Material Directo del Inventario.");
    }
}

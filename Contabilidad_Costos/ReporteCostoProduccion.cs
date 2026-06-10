class ReporteCostoProduccion
{
    private string nombreEmpresa;
    private string productoFabricado;
    private string periodo;
    private int    unidadesProducidas;
    private decimal materialDirecto;
    private decimal manoDeObraDirecta;
    private decimal costosIndirectosFabricacion;

    // ── Propiedades ───────────────────────────────────────────────────────

    public string NombreEmpresa
    {
        get { return nombreEmpresa; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El nombre de la empresa no puede quedar vacío.");
            else
                nombreEmpresa = value;
        }
    }

    public string ProductoFabricado
    {
        get { return productoFabricado; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El producto fabricado no puede quedar vacío.");
            else
                productoFabricado = value;
        }
    }

    public string Periodo
    {
        get { return periodo; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El período no puede quedar vacío.");
            else
                periodo = value;
        }
    }

    public int UnidadesProducidas
    {
        get { return unidadesProducidas; }
        set
        {
            if (value <= 0)
                Console.WriteLine("  Las unidades producidas deben ser mayor a cero.");
            else
                unidadesProducidas = value;
        }
    }

    public decimal MaterialDirecto
    {
        get { return materialDirecto; }
        set
        {
            if (value < 0)
                Console.WriteLine("  El material directo no puede ser negativo.");
            else
                materialDirecto = value;
        }
    }

    public decimal ManoDeObraDirecta
    {
        get { return manoDeObraDirecta; }
        set
        {
            if (value < 0)
                Console.WriteLine("  La mano de obra directa no puede ser negativa.");
            else
                manoDeObraDirecta = value;
        }
    }

    public decimal CostosIndirectosFabricacion
    {
        get { return costosIndirectosFabricacion; }
        set
        {
            if (value < 0)
                Console.WriteLine("  Los CIF no pueden ser negativos.");
            else
                costosIndirectosFabricacion = value;
        }
    }

    // ── Cálculos derivados ────────────────────────────────────────────────

    public decimal CostoTotalProduccion
    {
        get { return materialDirecto + manoDeObraDirecta + costosIndirectosFabricacion; }
    }

    public decimal CostoUnitario
    {
        get
        {
            if (unidadesProducidas <= 0) return 0;
            return CostoTotalProduccion / unidadesProducidas;
        }
    }

    // ── Constructor ───────────────────────────────────────────────────────

    public ReporteCostoProduccion()
    {
        nombreEmpresa               = "";
        productoFabricado           = "";
        periodo                     = "";
        unidadesProducidas          = 1;
        materialDirecto             = 0;
        manoDeObraDirecta           = 0;
        costosIndirectosFabricacion = 0;
    }

    // ── Reporte en tabla ──────────────────────────────────────────────────

    public void ImprimirReporte()
    {
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║            REPORTE DE COSTO DE PRODUCCIÓN                    ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");
        Console.WriteLine($"  Empresa  : {nombreEmpresa}");
        Console.WriteLine($"  Producto : {productoFabricado}");
        Console.WriteLine($"  Período  : {periodo}");
        Console.WriteLine();

        // ── Tabla principal de costos ─────────────────────────────────────
        var filasCostos = new List<(string, string)>
        {
            ("Material directo consumido",        $"C$ {materialDirecto,14:N2}"),
            ("Mano de obra directa",               $"C$ {manoDeObraDirecta,14:N2}"),
            ("Costos indirectos de fabricación",   $"C$ {costosIndirectosFabricacion,14:N2}")
        };

        TablaConsola.ImprimirResumen(
            titulo:         "Elemento del Costo",
            filas:          filasCostos,
            totalConcepto:  "COSTO TOTAL DE PRODUCCIÓN",
            totalMonto:     $"C$ {CostoTotalProduccion,14:N2}"
        );

        Console.WriteLine();

        // ── Tabla de costo unitario ───────────────────────────────────────
        var filasUnitario = new List<string[]>
        {
            new[] { "Costo total de producción",  $"C$ {CostoTotalProduccion:N2}", "" },
            new[] { "Unidades producidas",         unidadesProducidas.ToString("N0"), "" },
            new[] { "COSTO UNITARIO DE PRODUCCIÓN", "",  $"C$ {CostoUnitario:N2}" }
        };

        TablaConsola.Imprimir(
            new[] { "Concepto", "Valor", "Costo Unitario" },
            filasUnitario,
            alinDer: new[] { 1, 2 }
        );

        Console.WriteLine();

        // ── Tabla de flujo de información ─────────────────────────────────
        Console.WriteLine("  ▸ FLUJO DE INFORMACIÓN\n");

        var filasFluj = new List<string[]>
        {
            new[] { "Módulo de Inventario",  "Material directo consumido",      $"C$ {materialDirecto:N2}" },
            new[] { "Módulo de Nómina",      "Mano de obra directa",            $"C$ {manoDeObraDirecta:N2}" },
            new[] { "Módulo de CIF",         "Costos indirectos de fabricación", $"C$ {costosIndirectosFabricacion:N2}" },
            new[] { "Resultado",
                    $"MD + MOD + CIF  →  {unidadesProducidas} unidades",
                    $"C$ {CostoUnitario:N2} / u" }
        };

        TablaConsola.Imprimir(
            new[] { "Origen", "Dato aportado", "Monto / Resultado" },
            filasFluj,
            alinDer: new[] { 2 }
        );

        Console.WriteLine();
        Console.WriteLine("  " + new string('═', 62));
        Console.WriteLine($"  Costo total = C$ {materialDirecto:N2} + C$ {manoDeObraDirecta:N2} + C$ {costosIndirectosFabricacion:N2} = C$ {CostoTotalProduccion:N2}");
        Console.WriteLine($"  Costo unitario = C$ {CostoTotalProduccion:N2} / {unidadesProducidas} u = C$ {CostoUnitario:N2} por unidad");
        Console.WriteLine("  " + new string('═', 62));
    }
}

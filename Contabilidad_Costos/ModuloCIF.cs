class ModuloCIF
{
    private List<CostoIndirecto> costos;

    /// <summary>MOI proveniente del módulo de nómina (se asigna antes de imprimir o calcular).</summary>
    public decimal MOINomina { get; set; } = 0m;

    /// <summary>Comportamiento del costo de MOI tomado de Nómina (por defecto Fijo).</summary>
    public ComportamientoCosto ComportamientoMOINomina { get; set; } = ComportamientoCosto.Fijo;

    /// <summary>Materiales indirectos provenientes del módulo de inventario (se asigna antes de imprimir o calcular).</summary>
    public decimal MatIndirectosInventario { get; set; } = 0m;

    /// <summary>Comportamiento del costo de Mat. Indirectos tomado de Inventario (por defecto Variable).</summary>
    public ComportamientoCosto ComportamientoMatIndirectos { get; set; } = ComportamientoCosto.Variable;

    public ModuloCIF()
    {
        costos = new List<CostoIndirecto>();
    }

    // ── CRUD ──────────────────────────────────────────────────────────────

    public void AgregarCosto(CostoIndirecto c)
    {
        costos.Add(c);
        Console.WriteLine($"\n  ✔  CIF '{c.Concepto}' registrado correctamente.");
    }

    public void EliminarCosto(string concepto)
    {
        CostoIndirecto? encontrado = null;
        foreach (CostoIndirecto c in costos)
        {
            if (c.Concepto.ToUpper() == concepto.ToUpper())
            {
                encontrado = c;
                break;
            }
        }
        if (encontrado != null)
        {
            costos.Remove(encontrado);
            Console.WriteLine($"\n  ✔  CIF '{concepto}' eliminado.");
        }
        else
        {
            Console.WriteLine($"\n  ⚠  No se encontró el CIF '{concepto}'.");
        }
    }

    // ── Totales ───────────────────────────────────────────────────────────

    public decimal TotalMaterialesIndirectos()
    {
        decimal total = MatIndirectosInventario;
        foreach (CostoIndirecto c in costos)
            if (c.Tipo == TipoCIF.MaterialIndirecto)
                total += c.Monto;
        return total;
    }

    public decimal TotalMOIndirecta()
    {
        decimal total = MOINomina;
        foreach (CostoIndirecto c in costos)
            if (c.Tipo == TipoCIF.ManoDeObraIndirecta)
                total += c.Monto;
        return total;
    }

    public decimal TotalOtrosCIF()
    {
        decimal total = 0;
        foreach (CostoIndirecto c in costos)
            if (c.Tipo == TipoCIF.OtroCIF)
                total += c.Monto;
        return total;
    }

    public decimal TotalCIF()
    {
        decimal total = MOINomina + MatIndirectosInventario;
        foreach (CostoIndirecto c in costos)
            total += c.Monto;
        return total;
    }

    // ── Reporte en tabla ──────────────────────────────────────────────────

    public void ImprimirReporte()
    {
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║      MÓDULO DE COSTOS INDIRECTOS DE FABRICACIÓN (CIF)        ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");

        Console.WriteLine("\n  ▸ MATERIALES INDIRECTOS\n");
        if (MatIndirectosInventario > 0)
        {
            var filasInv = new List<string[]>
            {
                new[] { "Mat. Indirectos – Inventario", ComportamientoMatIndirectos.ToString(), "Producción", $"C$ {MatIndirectosInventario:N2}", "Calculado en inventario" }
            };
            TablaConsola.Imprimir(
                new[] { "Concepto", "Comportamiento", "Área", "Monto", "Observación" },
                filasInv,
                alinDer: new[] { 3 }
            );
        }
        ImprimirTablaCIF(TipoCIF.MaterialIndirecto);
        Console.WriteLine($"  Subtotal Mat. Indirectos:     C$ {TotalMaterialesIndirectos(),14:N2}");

        Console.WriteLine("\n  ▸ MANO DE OBRA INDIRECTA\n");
        if (MOINomina > 0)
        {
            var filasNomina = new List<string[]>
            {
                new[] { "MOI – Módulo de Nómina", ComportamientoMOINomina.ToString(), "Producción", $"C$ {MOINomina:N2}", "Calculado en nómina" }
            };
            TablaConsola.Imprimir(
                new[] { "Concepto", "Comportamiento", "Área", "Monto", "Observación" },
                filasNomina,
                alinDer: new[] { 3 }
            );
        }
        ImprimirTablaCIF(TipoCIF.ManoDeObraIndirecta);
        Console.WriteLine($"  Subtotal MOI:                 C$ {TotalMOIndirecta(),14:N2}");

        Console.WriteLine("\n  ▸ OTROS CIF\n");
        ImprimirTablaCIF(TipoCIF.OtroCIF);
        Console.WriteLine($"  Subtotal Otros CIF:           C$ {TotalOtrosCIF(),14:N2}");

        Console.WriteLine();
        Console.WriteLine($"  {"TOTAL CIF",-36}  C$ {TotalCIF(),14:N2}");
        Console.WriteLine("  " + new string('═', 62));
    }

    private void ImprimirTablaCIF(TipoCIF tipo)
    {
        var filas = new List<string[]>();
        foreach (CostoIndirecto c in costos)
        {
            if (c.Tipo == tipo)
            {
                filas.Add(new[]
                {
                    c.Concepto,
                    c.Comportamiento.ToString(),
                    c.AreaRelacionada,
                    $"C$ {c.Monto:N2}",
                    string.IsNullOrWhiteSpace(c.Observacion) ? "—" : c.Observacion
                });
            }
        }

        if (filas.Count == 0)
        {
            Console.WriteLine("  (Sin registros)\n");
            return;
        }

        TablaConsola.Imprimir(
            new[] { "Concepto", "Comportamiento", "Área", "Monto", "Observación" },
            filas,
            alinDer: new[] { 3 }
        );
    }
}
class ModuloInventario
{
    private List<Material> materiales;

    public ModuloInventario()
    {
        materiales = new List<Material>();
    }

    // ── CRUD ──────────────────────────────────────────────────────────────

    public void AgregarMaterial(Material m)
    {
        foreach (Material mat in materiales)
        {
            if (mat.Codigo.ToUpper() == m.Codigo.ToUpper())
            {
                Console.WriteLine($"\n  ⚠  Ya existe un material con el código '{m.Codigo}'.");
                return;
            }
        }
        materiales.Add(m);
        Console.WriteLine($"\n  ✔  Material '{m.Nombre}' registrado correctamente.");
    }

    public void EliminarMaterial(string codigo)
    {
        Material? encontrado = null;
        foreach (Material m in materiales)
        {
            if (m.Codigo.ToUpper() == codigo.ToUpper())
            {
                encontrado = m;
                break;
            }
        }
        if (encontrado != null)
        {
            materiales.Remove(encontrado);
            Console.WriteLine($"\n  ✔  Material '{codigo}' eliminado.");
        }
        else
        {
            Console.WriteLine($"\n  ⚠  No se encontró el código '{codigo}'.");
        }
    }

    // ── Totales ───────────────────────────────────────────────────────────

    public decimal TotalMaterialDirecto()
    {
        decimal total = 0;
        foreach (Material m in materiales)
            if (m.Tipo == TipoMaterial.Directo)
                total += m.CostoTotal;
        return total;
    }

    public decimal TotalMaterialIndirecto()
    {
        decimal total = 0;
        foreach (Material m in materiales)
            if (m.Tipo == TipoMaterial.Indirecto)
                total += m.CostoTotal;
        return total;
    }

    // ── Reporte en tabla ──────────────────────────────────────────────────

    public void ImprimirReporte()
    {
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║           MÓDULO DE INVENTARIO DE MATERIALES                 ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");

        // ── Materiales Directos ──────────────────────────────────────────
        Console.WriteLine("\n  ▸ MATERIALES DIRECTOS\n");
        ImprimirTablaMateriales(TipoMaterial.Directo);
        Console.WriteLine($"  Subtotal Material Directo:    C$ {TotalMaterialDirecto(),14:N2}");

        // ── Materiales Indirectos ────────────────────────────────────────
        Console.WriteLine("\n  ▸ MATERIALES INDIRECTOS  (→ se acumulan en CIF)\n");
        ImprimirTablaMateriales(TipoMaterial.Indirecto);
        Console.WriteLine($"  Subtotal Material Indirecto:  C$ {TotalMaterialIndirecto(),14:N2}");

        Console.WriteLine();
        Console.WriteLine($"  {"TOTAL INVENTARIO",-36}  C$ {TotalMaterialDirecto() + TotalMaterialIndirecto(),14:N2}");
        Console.WriteLine("  " + new string('═', 62));
    }

    private void ImprimirTablaMateriales(TipoMaterial tipo)
    {
        var filas = new List<string[]>();
        foreach (Material m in materiales)
        {
            if (m.Tipo == tipo)
            {
                filas.Add(new[]
                {
                    m.Codigo,
                    m.Nombre,
                    m.UnidadMedida,
                    m.CantidadDisponible.ToString("N2"),
                    m.CantidadConsumida.ToString("N2"),
                    $"C$ {m.CostoUnitario:N2}",
                    $"C$ {m.CostoTotal:N2}"
                });
            }
        }

        if (filas.Count == 0)
        {
            Console.WriteLine("  (Sin registros)\n");
            return;
        }

        TablaConsola.Imprimir(
            new[] { "Código", "Material", "Unidad", "Disponible", "Consumida", "C. Unitario", "C. Total" },
            filas,
            alinDer: new[] { 3, 4, 5, 6 }
        );
    }
}

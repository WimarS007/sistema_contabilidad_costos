class ModuloInventario
{
    private List<Material> materiales;
    private ModuloPEPS?    pepsDirecto;       // ← PEPS para materiales directos
    private ModuloPEPS?    pepsIndirecto;     // ← PEPS para materiales indirectos (¡nuevo!)

    // ── Constructor ────────────────────────────────────────────────────────

    public ModuloInventario(ModuloPEPS? pepsDirecto = null, ModuloPEPS? pepsIndirecto = null)
    {
        materiales          = new List<Material>();
        this.pepsDirecto    = pepsDirecto;
        this.pepsIndirecto  = pepsIndirecto;
    }

    // ── CRUD ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Agrega un material. Registra automáticamente la entrada
    /// (y la salida si ya hay consumo) en el kardex PEPS correspondiente:
    /// materiales Directos → pepsDirecto | Indirectos → pepsIndirecto.
    /// </summary>
    public void AgregarMaterial(Material m, string fecha = "")
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

        // ── Conexión con PEPS según tipo ──────────────────────────────────
        ModuloPEPS? peps = ObtenerPEPS(m.Tipo);
        string tipoLabel  = m.Tipo == TipoMaterial.Directo ? "Directo" : "Indirecto";

        if (peps != null)
        {
            string f = string.IsNullOrWhiteSpace(fecha) ? "—" : fecha;

            // 1. Registrar lote de entrada (inventario inicial / primera compra)
            if (m.CantidadDisponible > 0)
            {
                Console.WriteLine($"  → PEPS {tipoLabel}: entrada registrada " +
                                  $"({m.CantidadDisponible:N2} u × C$ {m.CostoUnitario:N2})");
                peps.RegistrarEntrada(f, $"Inv. inicial / {m.Nombre}",
                                      m.CantidadDisponible, m.CostoUnitario);
            }

            // 2. Si ya hay consumo inicial, registrar salida PEPS de inmediato
            if (m.CantidadConsumida > 0)
            {
                Console.WriteLine($"  → PEPS {tipoLabel}: salida registrada " +
                                  $"({m.CantidadConsumida:N2} u consumidas)");
                peps.RegistrarSalida(f, $"Consumo inicial / {m.Nombre}", m.CantidadConsumida);
            }
        }
    }

    /// <summary>
    /// Registra una compra adicional del mismo material:
    /// suma unidades al stock y agrega un nuevo lote PEPS con el precio nuevo.
    /// Aplica tanto a materiales Directos como Indirectos.
    /// </summary>
    public void RegistrarCompraAdicional(string codigo, decimal cantidad,
                                         decimal costoUnit, string fecha)
    {
        Material? mat = Buscar(codigo);
        if (mat == null)
        {
            Console.WriteLine($"\n  ⚠  No se encontró el código '{codigo}'.");
            return;
        }

        mat.CantidadDisponible += cantidad;
        Console.WriteLine($"\n  ✔  Compra adicional: +{cantidad:N2} u → stock {mat.CantidadDisponible:N2} u");

        // ── Conexión con PEPS según tipo ──────────────────────────────────
        ModuloPEPS? peps = ObtenerPEPS(mat.Tipo);
        string tipoLabel  = mat.Tipo == TipoMaterial.Directo ? "Directo" : "Indirecto";

        if (peps != null)
        {
            string f = string.IsNullOrWhiteSpace(fecha) ? "—" : fecha;
            Console.WriteLine($"  → PEPS {tipoLabel}: nuevo lote registrado " +
                              $"({cantidad:N2} u × C$ {costoUnit:N2})");
            peps.RegistrarEntrada(f, $"Compra / {mat.Nombre}", cantidad, costoUnit);
        }
    }

    /// <summary>
    /// Registra el consumo de un material en producción:
    /// actualiza CantidadConsumida y descarga la salida del kardex PEPS por FIFO.
    /// Aplica tanto a materiales Directos como Indirectos.
    /// </summary>
    public void RegistrarConsumo(string codigo, decimal cantidad, string fecha)
    {
        Material? mat = Buscar(codigo);
        if (mat == null)
        {
            Console.WriteLine($"\n  ⚠  No se encontró el código '{codigo}'.");
            return;
        }

        decimal nueva = mat.CantidadConsumida + cantidad;
        if (nueva > mat.CantidadDisponible)
        {
            Console.WriteLine($"\n  ⚠  Consumo excede disponible " +
                              $"({mat.CantidadDisponible:N2} u disponibles, " +
                              $"{mat.CantidadConsumida:N2} u ya consumidas).");
            return;
        }

        mat.CantidadConsumida = nueva;
        Console.WriteLine($"\n  ✔  Consumo registrado: {cantidad:N2} u de '{mat.Nombre}'");

        // ── Conexión con PEPS según tipo ──────────────────────────────────
        ModuloPEPS? peps = ObtenerPEPS(mat.Tipo);
        string tipoLabel  = mat.Tipo == TipoMaterial.Directo ? "Directo" : "Indirecto";

        if (peps != null)
        {
            string f = string.IsNullOrWhiteSpace(fecha) ? "—" : fecha;
            Console.WriteLine($"  → PEPS {tipoLabel}: salida FIFO registrada ({cantidad:N2} u)");
            peps.RegistrarSalida(f, $"Consumo / {mat.Nombre}", cantidad);
        }
    }

    public void EliminarMaterial(string codigo)
    {
        Material? encontrado = Buscar(codigo);
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

    // ── Totales ────────────────────────────────────────────────────────────

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

    // ── Reporte en tabla ───────────────────────────────────────────────────

    public void ImprimirReporte()
    {
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║           MÓDULO DE INVENTARIO DE MATERIALES                 ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════╝");

        Console.WriteLine("\n  ▸ MATERIALES DIRECTOS  (entradas/salidas → kardex PEPS Directo)\n");
        ImprimirTablaMateriales(TipoMaterial.Directo);
        Console.WriteLine($"  Subtotal Material Directo:    C$ {TotalMaterialDirecto(),14:N2}");

        Console.WriteLine("\n  ▸ MATERIALES INDIRECTOS  (entradas/salidas → kardex PEPS Indirecto)\n");
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

    // ── Helpers privados ───────────────────────────────────────────────────

    /// <summary>Devuelve la instancia PEPS correcta según el tipo de material.</summary>
    private ModuloPEPS? ObtenerPEPS(TipoMaterial tipo)
        => tipo == TipoMaterial.Directo ? pepsDirecto : pepsIndirecto;

    private Material? Buscar(string codigo)
    {
        foreach (Material m in materiales)
            if (m.Codigo.ToUpper() == codigo.ToUpper())
                return m;
        return null;
    }
}

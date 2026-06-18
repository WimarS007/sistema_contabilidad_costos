class Program
{
    static ModuloPEPS       peps           = new ModuloPEPS();          // ← materiales directos
    static ModuloPEPS       pepsIndirecto  = new ModuloPEPS();          // ← materiales indirectos
    static ModuloInventario inventario     = new ModuloInventario(peps, pepsIndirecto); // ← ambos conectados
    static ModuloNomina     nomina         = new ModuloNomina();
    static ModuloCIF        cif            = new ModuloCIF();
    static string nombreEmpresa            = "";
    static string productoBase             = "";
    static bool   sistemaConfigurado       = false;

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        MenuPrincipal();
    }

    // ── MENÚ PRINCIPAL ────────────────────────────────────────────────────

    static void MenuPrincipal()
    {
        while (true)
        {
            Encabezado("SISTEMA DE COSTOS DE PRODUCCIÓN");
            if (sistemaConfigurado)
                Console.WriteLine($"  Empresa: {nombreEmpresa}  |  Producto: {productoBase}\n");

            Console.WriteLine("  [1] Configurar empresa y producto");
            Console.WriteLine("  [2] Módulo de Inventario");
            Console.WriteLine("  [3] Módulo de Nómina");
            Console.WriteLine("  [4] Módulo de CIF");
            Console.WriteLine("  [5] Módulo PEPS (FIFO)");
            Console.WriteLine("  [6] Generar Reporte Final");
            Console.WriteLine("  [0] Salir");

            string opcion = Leer("\nSelecciona una opción");

            if      (opcion == "1") ConfigurarEmpresa();
            else if (opcion == "2") MenuInventario();
            else if (opcion == "3") MenuNomina();
            else if (opcion == "4") MenuCIF();
            else if (opcion == "5") MenuPEPS();
            else if (opcion == "6") MenuReporte();
            else if (opcion == "0") { Console.WriteLine("\n  ¡Hasta luego!\n"); break; }
            else Console.WriteLine("\n  Opción no válida.");
        }
    }

    // ── CONFIGURAR EMPRESA ────────────────────────────────────────────────

    static void ConfigurarEmpresa()
    {
        Encabezado("CONFIGURAR EMPRESA");
        nombreEmpresa      = LeerObligatorio("Nombre de la empresa");
        productoBase       = LeerObligatorio("Producto a costear");
        sistemaConfigurado = true;
        Console.WriteLine($"\n  ✔  Sistema configurado: {nombreEmpresa} — {productoBase}");
        Pausa();
    }

    // ── INVENTARIO ────────────────────────────────────────────────────────

    static void MenuInventario()
    {
        if (!RequiereSistema()) return;
        while (true)
        {
            Encabezado("MÓDULO DE INVENTARIO");
            inventario.ImprimirReporte();
            Console.WriteLine("\n  [1] Agregar material  (inv. inicial / primera compra  → PEPS)");
            Console.WriteLine("  [2] Registrar compra adicional                        → PEPS");
            Console.WriteLine("  [3] Registrar consumo de material                     → PEPS");
            Console.WriteLine("  [4] Eliminar material");
            Console.WriteLine("  [0] Volver");

            string op = Leer("\nOpción");
            if      (op == "1") AgregarMaterial();
            else if (op == "2") CompraAdicionalMaterial();
            else if (op == "3") ConsumirMaterial();
            else if (op == "4") EliminarMaterial();
            else if (op == "0") break;
            else Console.WriteLine("\n  Opción no válida.");
        }
    }

    static void AgregarMaterial()
    {
        Encabezado("AGREGAR MATERIAL  (Inventario Inicial / Primera Compra)");
        string   fecha           = LeerObligatorio("Fecha del movimiento  (ej: 01-ene-2025)");
        Material m               = new Material();
        m.Codigo             = LeerObligatorio("Código  (ej: MD-001)");
        m.Nombre             = LeerObligatorio("Nombre del material");
        m.Tipo               = ElegirTipoMaterial();
        m.UnidadMedida       = LeerObligatorio("Unidad de medida  (tabla, libra, metro…)");
        m.CantidadDisponible = LeerDecimalPositivo("Cantidad disponible (inventario inicial)");
        m.CantidadConsumida  = LeerDecimal("Cantidad ya consumida  (0 si es inv. inicial puro)");
        m.CostoUnitario      = LeerDecimalPositivo("Costo unitario (C$)");
        inventario.AgregarMaterial(m, fecha);
        Console.WriteLine($"  Costo total calculado: C$ {m.CostoTotal:N2}");
        Pausa();
    }

    static void CompraAdicionalMaterial()
    {
        Encabezado("REGISTRAR COMPRA ADICIONAL  (→ nuevo lote PEPS)");
        string  codigo    = LeerObligatorio("Código del material");
        string  fecha     = LeerObligatorio("Fecha de la compra   (ej: 15-ene-2025)");
        decimal cantidad  = LeerDecimalPositivo("Cantidad comprada");
        decimal costoUnit = LeerDecimalPositivo("Costo unitario de esta compra (C$)");
        inventario.RegistrarCompraAdicional(codigo, cantidad, costoUnit, fecha);
        Pausa();
    }

    static void ConsumirMaterial()
    {
        Encabezado("REGISTRAR CONSUMO DE MATERIAL  (→ salida FIFO en PEPS)");
        string  codigo   = LeerObligatorio("Código del material");
        string  fecha    = LeerObligatorio("Fecha del consumo   (ej: 20-ene-2025)");
        decimal cantidad = LeerDecimalPositivo("Cantidad consumida");
        inventario.RegistrarConsumo(codigo, cantidad, fecha);
        Pausa();
    }

    static void EliminarMaterial()
    {
        string codigo = LeerObligatorio("Código del material a eliminar");
        inventario.EliminarMaterial(codigo);
        Pausa();
    }

    // ── NÓMINA ────────────────────────────────────────────────────────────

    static void MenuNomina()
    {
        if (!RequiereSistema()) return;
        while (true)
        {
            Encabezado("MÓDULO DE NÓMINA");
            nomina.ImprimirReporte();
            Console.WriteLine("\n  [1] Agregar trabajador");
            Console.WriteLine("  [2] Eliminar trabajador");
            Console.WriteLine("  [0] Volver");

            string op = Leer("\nOpción");
            if      (op == "1") AgregarTrabajador();
            else if (op == "2") EliminarTrabajador();
            else if (op == "0") break;
            else Console.WriteLine("\n  Opción no válida.");
        }
    }

    static void AgregarTrabajador()
    {
        Encabezado("AGREGAR TRABAJADOR");

        Trabajador t = new Trabajador();

        Console.Write("  > No. INSS (opcional, Enter para omitir): ");
        t.NoINSS = Console.ReadLine()?.Trim() ?? "";

        t.Nombre          = LeerObligatorio("Nombre completo");
        t.Cargo           = LeerObligatorio("Cargo");
        t.Area            = LeerObligatorio("Área");
        t.Clasificacion   = ElegirClasificacion();
        t.HorasOrdinarias = LeerDecimal("Horas ordinarias del período");
        t.SalarioMensual  = LeerDecimal("Salario mensual (C$)");

        t.Bono            = LeerDecimal("Bono (C$, 0 si no aplica)");
        t.HorasExtras     = LeerDecimal("Horas extras (0 si no aplica)");

        if (t.HorasExtras > 0)
        {
            Console.WriteLine($"  Tarifa hora extra calculada (sal.mensual / 240 × 2): C$ {t.TarifaHoraExtra:N2}");
            Console.WriteLine($"  Ingreso por horas extras: C$ {t.IngresoHorasExtras:N2}");
        }

        t.Antiguedad = LeerDecimal("Antigüedad (C$, 0 si no aplica)");

        Console.WriteLine();
        Console.WriteLine($"  ── Resumen ─────────────────────────────────────────");
        Console.WriteLine($"  Total Ingresos:     C$ {t.TotalIngresos:N2}");
        Console.WriteLine($"  INSS Laboral (7%):  C$ {t.INSSLaboral:N2}");
        Console.WriteLine($"  IR Laboral:         C$ {t.IRLaboral:N2}");
        Console.WriteLine($"  Total Deducciones:  C$ {t.TotalDeducciones:N2}");
        Console.WriteLine($"  Neto a Recibir:     C$ {t.NetoARecibir:N2}");
        Console.WriteLine($"  INSS Patronal:      C$ {t.INSSPatronal:N2}");
        Console.WriteLine($"  INATEC:             C$ {t.INATEC:N2}");
        Console.WriteLine($"  Vacaciones:         C$ {t.Vacaciones:N2}");
        Console.WriteLine($"  Treceavo mes:       C$ {t.TreceavoMes:N2}");

        nomina.AgregarTrabajador(t);
        Pausa();
    }

    static void EliminarTrabajador()
    {
        string nombre = LeerObligatorio("Nombre del trabajador a eliminar");
        nomina.EliminarTrabajador(nombre);
        Pausa();
    }

    // ── CIF ───────────────────────────────────────────────────────────────

    static void MenuCIF()
    {
        if (!RequiereSistema()) return;
        while (true)
        {
            Encabezado("MÓDULO DE CIF");
            cif.MOINomina = nomina.TotalMOI();

            // Usar CostoVentas PEPS de indirectos si hay movimientos; si no, el total del inventario
            cif.MatIndirectosInventario = pepsIndirecto.TieneMovimientos
                ? pepsIndirecto.CostoVentas
                : inventario.TotalMaterialIndirecto();

            cif.ImprimirReporte();

            if (pepsIndirecto.TieneMovimientos)
                Console.WriteLine($"\n  ℹ  Material Indirecto tomado del Costo de Ventas PEPS: " +
                                  $"C$ {pepsIndirecto.CostoVentas:N2}");

            Console.WriteLine("\n  [1] Agregar costo indirecto");
            Console.WriteLine("  [2] Eliminar costo indirecto");
            Console.WriteLine("  [0] Volver");

            string op = Leer("\nOpción");
            if      (op == "1") AgregarCIF();
            else if (op == "2") EliminarCIF();
            else if (op == "0") break;
            else Console.WriteLine("\n  Opción no válida.");
        }
    }

    static void AgregarCIF()
    {
        Encabezado("AGREGAR COSTO INDIRECTO DE FABRICACIÓN");
        CostoIndirecto c = new CostoIndirecto();
        c.Concepto        = LeerObligatorio("Concepto  (ej: Energía eléctrica de planta)");
        c.Tipo            = ElegirTipoCIF();
        c.Comportamiento  = ElegirComportamiento();
        c.Monto           = LeerDecimal("Monto (C$)");
        c.AreaRelacionada = LeerObligatorio("Área relacionada");
        Console.Write("  > Observación (opcional, Enter para omitir): ");
        c.Observacion     = Console.ReadLine() ?? "";
        cif.AgregarCosto(c);
        Pausa();
    }

    static void EliminarCIF()
    {
        string concepto = LeerObligatorio("Concepto a eliminar");
        cif.EliminarCosto(concepto);
        Pausa();
    }

    // ── PEPS ──────────────────────────────────────────────────────────────

    static void MenuPEPS()
    {
        if (!RequiereSistema()) return;
        while (true)
        {
            Encabezado("MÓDULO PEPS (FIFO) — KARDEX DE INVENTARIO");
            Console.WriteLine("  Las entradas y salidas se generan automáticamente desde");
            Console.WriteLine("  el Módulo de Inventario (opciones 1, 2 y 3 de ese menú).\n");

            // ── Kardex materiales directos ────────────────────────────────
            peps.ImprimirKardex("MATERIALES DIRECTOS        ");

            // ── Kardex materiales indirectos ──────────────────────────────
            pepsIndirecto.ImprimirKardex("MATERIALES INDIRECTOS      ");

            Console.WriteLine("\n  [1] Reiniciar kardex PEPS Directos");
            Console.WriteLine("  [2] Reiniciar kardex PEPS Indirectos");
            Console.WriteLine("  [3] Reiniciar AMBOS kardex");
            Console.WriteLine("  [0] Volver");

            string op = Leer("\nOpción");
            if      (op == "1") ConfirmarLimpiezaPEPS(peps,          "Directos");
            else if (op == "2") ConfirmarLimpiezaPEPS(pepsIndirecto, "Indirectos");
            else if (op == "3") { ConfirmarLimpiezaPEPS(peps, "Directos"); ConfirmarLimpiezaPEPS(pepsIndirecto, "Indirectos"); }
            else if (op == "0") break;
            else Console.WriteLine("\n  Opción no válida.");
        }
    }

    static void ConfirmarLimpiezaPEPS(ModuloPEPS modulo, string nombre)
    {
        Console.Write($"\n  ¿Seguro que deseas eliminar el kardex PEPS {nombre}? [S/N]: ");
        string resp = Console.ReadLine()?.Trim().ToUpper() ?? "";
        if (resp == "S")
            modulo.LimpiarMovimientos();
        else
            Console.WriteLine("  Operación cancelada.");
        Pausa();
    }

    // ── REPORTE FINAL ─────────────────────────────────────────────────────

    static void MenuReporte()
    {
        if (!RequiereSistema()) return;
        Encabezado("REPORTE FINAL");

        ReporteCostoProduccion reporte = new ReporteCostoProduccion();
        reporte.NombreEmpresa               = nombreEmpresa;
        reporte.ProductoFabricado           = productoBase;
        reporte.Periodo                     = LeerObligatorio("Período  (ej: Enero 2025)");
        reporte.UnidadesProducidas          = (int)LeerDecimal("Unidades producidas");
        reporte.MaterialDirecto             = inventario.TotalMaterialDirecto();
        reporte.ManoDeObraDirecta           = nomina.TotalMOD();
        cif.MOINomina                       = nomina.TotalMOI();
        cif.MatIndirectosInventario         = pepsIndirecto.TieneMovimientos
                                                ? pepsIndirecto.CostoVentas
                                                : inventario.TotalMaterialIndirecto();
        reporte.CostosIndirectosFabricacion = cif.TotalCIF();

        // Opción: usar Costo de Ventas PEPS de directos como Material Directo
        if (peps.TieneMovimientos)
        {
            Console.WriteLine($"\n  ℹ  PEPS Directos tiene movimientos.");
            Console.WriteLine($"     Costo de Ventas PEPS Directos:  C$ {peps.CostoVentas:N2}");
            Console.WriteLine($"     Material Directo actual:         C$ {reporte.MaterialDirecto:N2}");
            Console.Write("  ¿Usar Costo de Ventas PEPS como Material Directo? [S/N]: ");
            string resp = Console.ReadLine()?.Trim().ToUpper() ?? "";
            if (resp == "S")
            {
                reporte.MaterialDirecto = peps.CostoVentas;
                Console.WriteLine("  ✔  Material Directo sustituido por Costo de Ventas PEPS Directos.");
            }
        }

        // Opción: usar Costo de Ventas PEPS de indirectos como Material Indirecto en CIF
        if (pepsIndirecto.TieneMovimientos)
        {
            Console.WriteLine($"\n  ℹ  PEPS Indirectos tiene movimientos.");
            Console.WriteLine($"     Costo de Ventas PEPS Indirectos: C$ {pepsIndirecto.CostoVentas:N2}");
            Console.WriteLine($"     Mat. Indirecto en CIF actual:     C$ {cif.MatIndirectosInventario:N2}");
            Console.Write("  ¿Usar Costo de Ventas PEPS como Mat. Indirecto en CIF? [S/N]: ");
            string resp2 = Console.ReadLine()?.Trim().ToUpper() ?? "";
            if (resp2 == "S")
            {
                cif.MatIndirectosInventario         = pepsIndirecto.CostoVentas;
                reporte.CostosIndirectosFabricacion = cif.TotalCIF();
                Console.WriteLine("  ✔  Mat. Indirecto sustituido por Costo de Ventas PEPS Indirectos.");
            }
        }

        reporte.ImprimirReporte();

        // Resúmenes PEPS vinculados al reporte
        if (peps.TieneMovimientos)
            peps.ImprimirResumenParaReporte("materiales directos consumidos");

        if (pepsIndirecto.TieneMovimientos)
            pepsIndirecto.ImprimirResumenParaReporte("materiales indirectos consumidos");

        Pausa();
    }

    // ── HELPERS DE SELECCIÓN ──────────────────────────────────────────────

    static TipoMaterial ElegirTipoMaterial()
    {
        Console.WriteLine("\n  Tipo de material:");
        Console.WriteLine("  [1] Directo");
        Console.WriteLine("  [2] Indirecto");
        while (true)
        {
            string op = Leer("Opción");
            if (op == "1") return TipoMaterial.Directo;
            if (op == "2") return TipoMaterial.Indirecto;
            Console.WriteLine("  Elige 1 o 2.");
        }
    }

    static ClasificacionLaboral ElegirClasificacion()
    {
        Console.WriteLine("\n  Clasificación contable:");
        Console.WriteLine("  [1] Mano de Obra Directa  (MOD)");
        Console.WriteLine("  [2] Mano de Obra Indirecta (MOI)");
        Console.WriteLine("  [3] Gasto de Venta");
        Console.WriteLine("  [4] Gasto Administrativo");
        while (true)
        {
            string op = Leer("Opción");
            if (op == "1") return ClasificacionLaboral.ManoDeObraDirecta;
            if (op == "2") return ClasificacionLaboral.ManoDeObraIndirecta;
            if (op == "3") return ClasificacionLaboral.GastoDeVenta;
            if (op == "4") return ClasificacionLaboral.GastoAdministrativo;
            Console.WriteLine("  Elige entre 1 y 4.");
        }
    }

    static TipoCIF ElegirTipoCIF()
    {
        Console.WriteLine("\n  Tipo de CIF:");
        Console.WriteLine("  [1] Material Indirecto");
        Console.WriteLine("  [2] Mano de Obra Indirecta");
        Console.WriteLine("  [3] Otro CIF");
        while (true)
        {
            string op = Leer("Opción");
            if (op == "1") return TipoCIF.MaterialIndirecto;
            if (op == "2") return TipoCIF.ManoDeObraIndirecta;
            if (op == "3") return TipoCIF.OtroCIF;
            Console.WriteLine("  Elige entre 1 y 3.");
        }
    }

    static ComportamientoCosto ElegirComportamiento()
    {
        Console.WriteLine("\n  Comportamiento del costo:");
        Console.WriteLine("  [1] Fijo");
        Console.WriteLine("  [2] Variable");
        Console.WriteLine("  [3] Mixto");
        while (true)
        {
            string op = Leer("Opción");
            if (op == "1") return ComportamientoCosto.Fijo;
            if (op == "2") return ComportamientoCosto.Variable;
            if (op == "3") return ComportamientoCosto.Mixto;
            Console.WriteLine("  Elige entre 1 y 3.");
        }
    }

    // ── HELPERS DE LECTURA ────────────────────────────────────────────────

    static string Leer(string mensaje)
    {
        Console.Write($"  > {mensaje}: ");
        return Console.ReadLine()?.Trim() ?? "";
    }

    static string LeerObligatorio(string mensaje)
    {
        while (true)
        {
            string val = Leer(mensaje);
            if (!string.IsNullOrWhiteSpace(val)) return val;
            Console.WriteLine("  Este campo no puede quedar vacío.");
        }
    }

    static decimal LeerDecimal(string mensaje)
    {
        while (true)
        {
            string raw = Leer(mensaje);
            decimal val;
            bool ok = decimal.TryParse(
                raw.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out val);
            if (ok && val >= 0) return val;
            Console.WriteLine("  Ingresa un número válido (mayor o igual a 0).");
        }
    }

    static decimal LeerDecimalPositivo(string mensaje)
    {
        while (true)
        {
            string raw = Leer(mensaje);
            decimal val;
            bool ok = decimal.TryParse(
                raw.Replace(",", "."),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out val);
            if (ok && val > 0) return val;
            Console.WriteLine("  Ingresa un número mayor a cero.");
        }
    }

    static bool RequiereSistema()
    {
        if (!sistemaConfigurado)
        {
            Console.WriteLine("\n  Primero debes configurar la empresa (opción 1).");
            Pausa();
            return false;
        }
        return true;
    }

    static void Encabezado(string titulo)
    {
        Console.Clear();
        Console.WriteLine($"\n  {new string('═', 62)}");
        Console.WriteLine($"  {titulo}");
        Console.WriteLine($"  {new string('═', 62)}\n");
    }

    static void Pausa()
    {
        Console.Write("\n  Presiona Enter para continuar...");
        Console.ReadLine();
    }
}

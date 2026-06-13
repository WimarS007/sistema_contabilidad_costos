class ModuloNomina
{
    private List<Trabajador> trabajadores;

    public ModuloNomina()
    {
        trabajadores = new List<Trabajador>();
    }

    // ── CRUD ──────────────────────────────────────────────────────────────

    public void AgregarTrabajador(Trabajador t)
    {
        trabajadores.Add(t);
        Console.WriteLine($"\n  ✔  Trabajador '{t.Nombre}' registrado correctamente.");
    }

    public void EliminarTrabajador(string nombre)
    {
        Trabajador? encontrado = null;
        foreach (Trabajador t in trabajadores)
            if (t.Nombre.ToUpper() == nombre.ToUpper()) { encontrado = t; break; }

        if (encontrado != null)
        {
            trabajadores.Remove(encontrado);
            Console.WriteLine($"\n  ✔  Trabajador '{nombre}' eliminado.");
        }
        else
        {
            Console.WriteLine($"\n  ⚠  No se encontró al trabajador '{nombre}'.");
        }
    }

    // ── Totales para el reporte final ─────────────────────────────────────

    /// <summary>Total ingresos MOD (alimenta el reporte de costos).</summary>
    public decimal TotalMOD()
    {
        decimal total = 0;
        foreach (Trabajador t in trabajadores)
            if (t.Clasificacion == ClasificacionLaboral.ManoDeObraDirecta)
                total += t.TotalIngresos;
        return total;
    }

    public decimal TotalMOI()
    {
        decimal total = 0;
        foreach (Trabajador t in trabajadores)
            if (t.Clasificacion == ClasificacionLaboral.ManoDeObraIndirecta)
                total += t.TotalIngresos;
        return total;
    }

    public decimal TotalNomina()
    {
        decimal total = 0;
        foreach (Trabajador t in trabajadores)
            total += t.TotalIngresos;
        return total;
    }

    // ── Totales de deducciones y prestaciones ─────────────────────────────

    private decimal SumaINSSLaboral()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.INSSLaboral; return s;
    }
    private decimal SumaIRLaboral()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.IRLaboral; return s;
    }
    private decimal SumaTotalDeducciones()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.TotalDeducciones; return s;
    }
    private decimal SumaNetoARecibir()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.NetoARecibir; return s;
    }
    private decimal SumaINSSPatronal()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.INSSPatronal; return s;
    }
    private decimal SumaINATEC()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.INATEC; return s;
    }
    private decimal SumaVacaciones()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.Vacaciones; return s;
    }
    private decimal SumaTreceavoMes()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.TreceavoMes; return s;
    }
    private decimal SumaTotalPrestaciones()
    {
        decimal s = 0; foreach (Trabajador t in trabajadores) s += t.TotalPrestaciones; return s;
    }

    // ── REPORTE COMPLETO ──────────────────────────────────────────────────

    public void ImprimirReporte()
    {
        Console.WriteLine();
        Console.WriteLine("  ╔══════════════════════════════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("  ║                         MÓDULO DE NÓMINA DE PRODUCCIÓN                                  ║");
        Console.WriteLine("  ╚══════════════════════════════════════════════════════════════════════════════════════════╝");

        if (trabajadores.Count == 0)
        {
            Console.WriteLine("\n  (Sin trabajadores registrados)");
            Console.WriteLine("  " + new string('═', 90));
            return;
        }

        // ── 1. Tabla de Remuneración Bruta ────────────────────────────────
        Console.WriteLine("\n  ▸ REMUNERACIÓN BRUTA\n");
        {
            var filas = new List<string[]>();
            foreach (Trabajador t in trabajadores)
            {
                filas.Add(new[]
                {
                    t.NoINSS == "" ? "—" : t.NoINSS,
                    t.Nombre,
                    t.Cargo == "" ? "—" : t.Cargo,
                    ClasifLabel(t.Clasificacion),
                    $"C$ {t.SalarioMensual:N2}",
                    $"C$ {t.Bono:N2}",
                    t.HorasExtras > 0 ? t.HorasExtras.ToString("N1") : "0",
                    $"C$ {t.IngresoHorasExtras:N2}",
                    $"C$ {t.Antiguedad:N2}",
                    $"C$ {t.TotalIngresos:N2}"
                });
            }
            // Fila de totales
            filas.Add(new[]
            {
                "", "TOTALES", "", "",
                $"C$ {trabajadores.Sum(t=>t.SalarioMensual):N2}",
                $"C$ {trabajadores.Sum(t=>t.Bono):N2}",
                "",
                $"C$ {trabajadores.Sum(t=>t.IngresoHorasExtras):N2}",
                $"C$ {trabajadores.Sum(t=>t.Antiguedad):N2}",
                $"C$ {TotalNomina():N2}"
            });

            TablaConsola.Imprimir(
                new[] { "No. INSS", "Nombre", "Cargo", "Clasificación",
                        "Sal. Mensual", "Bono",
                        "H. Extra", "Ingr. H.E.", "Antigüedad", "Total Ingresos" },
                filas,
                alinDer: new[] { 4, 5, 6, 7, 8, 9 }
            );
        }

        // ── 2. Tabla de Deducciones ───────────────────────────────────────
        Console.WriteLine("\n  ▸ DEDUCCIONES\n");
        {
            var filas = new List<string[]>();
            foreach (Trabajador t in trabajadores)
            {
                filas.Add(new[]
                {
                    t.Nombre,
                    $"C$ {t.TotalIngresos:N2}",
                    $"C$ {t.INSSLaboral:N2}",
                    $"C$ {t.BaseImponibleIR:N2}",
                    $"C$ {t.IRLaboral:N2}",
                    $"C$ {t.TotalDeducciones:N2}",
                    $"C$ {t.NetoARecibir:N2}"
                });
            }
            filas.Add(new[]
            {
                "TOTALES",
                $"C$ {TotalNomina():N2}",
                $"C$ {SumaINSSLaboral():N2}",
                "",
                $"C$ {SumaIRLaboral():N2}",
                $"C$ {SumaTotalDeducciones():N2}",
                $"C$ {SumaNetoARecibir():N2}"
            });

            TablaConsola.Imprimir(
                new[] { "Nombre", "Total Ingresos", "INSS Lab. (7%)",
                        "Base Imp. IR", "IR Laboral", "Total Deduc.", "Neto a Recibir" },
                filas,
                alinDer: new[] { 1, 2, 3, 4, 5, 6 }
            );
        }

        // ── 3. Tabla de Prestaciones Sociales ─────────────────────────────
        Console.WriteLine("\n  ▸ PRESTACIONES SOCIALES  (cargo al empleador)\n");
        {
            var filas = new List<string[]>();
            foreach (Trabajador t in trabajadores)
            {
                filas.Add(new[]
                {
                    t.Nombre,
                    $"C$ {t.TotalIngresos:N2}",
                    $"C$ {t.INSSPatronal:N2}",
                    $"C$ {t.INATEC:N2}",
                    $"C$ {t.Vacaciones:N2}",
                    $"C$ {t.TreceavoMes:N2}",
                    $"C$ {t.TotalPrestaciones:N2}"
                });
            }
            filas.Add(new[]
            {
                "TOTALES",
                $"C$ {TotalNomina():N2}",
                $"C$ {SumaINSSPatronal():N2}",
                $"C$ {SumaINATEC():N2}",
                $"C$ {SumaVacaciones():N2}",
                $"C$ {SumaTreceavoMes():N2}",
                $"C$ {SumaTotalPrestaciones():N2}"
            });

            TablaConsola.Imprimir(
                new[] { "Nombre", "Total Ingresos",
                        "INSS Patron. (22.5%)", "INATEC (2%)",
                        "Vacaciones (8.33%)", "Treceavo (8.33%)", "Total Prestaciones" },
                filas,
                alinDer: new[] { 1, 2, 3, 4, 5, 6 }
            );
        }

        // ── 4. Asiento contable: nómina por pagar ─────────────────────────
        Console.WriteLine("\n  ▸ ASIENTO 1 – REGISTRO DE LA NÓMINA\n");
        ImprimirAsientoNomina();

        // ── 5. Asiento contable: prestaciones al empleador ────────────────
        Console.WriteLine("\n  ▸ ASIENTO 2 – REGISTRO DE PRESTACIONES AL EMPLEADOR\n");
        ImprimirAsientoPrestaciones();

        // ── 6. Tasas de referencia ────────────────────────────────────────
        Console.WriteLine("\n  ▸ TASAS APLICADAS\n");
        ImprimirTasas();

        Console.WriteLine();
        Console.WriteLine($"  {"TOTAL NÓMINA (Total Ingresos)",-42}  C$ {TotalNomina(),14:N2}");
        Console.WriteLine($"  {"  MOD",-42}  C$ {TotalMOD(),14:N2}");
        Console.WriteLine($"  {"  MOI (→ CIF)",-42}  C$ {TotalMOI(),14:N2}");
        Console.WriteLine("  " + new string('═', 62));
    }

    // ── Asiento 1: Nómina por pagar ───────────────────────────────────────
    private void ImprimirAsientoNomina()
    {
        decimal totalINSSLab = SumaINSSLaboral();
        decimal totalIR      = SumaIRLaboral();
        decimal totalDeduc   = SumaTotalDeducciones();
        decimal totalNeto    = SumaNetoARecibir();
        decimal totalIngr    = TotalNomina();

        var filas = new List<string[]>
        {
            new[] { "Nómina por pagar",          $"C$ {totalIngr:N2}",  "",                     "" },
            new[] { "  Retenciones por pagar",    "",                    "",                     "" },
            new[] { "    INSS Laboral por pagar", "",                    $"C$ {totalINSSLab:N2}", "" },
            new[] { "    IR por pagar",           "",                    $"C$ {totalIR:N2}",      "" },
            new[] { "  Banco (neto desembolsado)","",                    "",                     $"C$ {totalNeto:N2}" }
        };

        TablaConsola.Imprimir(
            new[] { "Cuenta", "DEBE", "PARCIAL (Haber)", "HABER" },
            filas,
            alinDer: new[] { 1, 2, 3 }
        );

        Console.WriteLine($"  Comprobación DEBE = HABER: C$ {totalIngr:N2} = C$ {totalDeduc:N2} + C$ {totalNeto:N2}");
    }

    // ── Asiento 2: Prestaciones al empleador ──────────────────────────────
    private void ImprimirAsientoPrestaciones()
    {
        decimal modINSSP  = 0; decimal modINAT = 0; decimal modVac = 0; decimal modTrec = 0;
        decimal moiINSSP  = 0; decimal moiINAT = 0; decimal moiVac = 0; decimal moiTrec = 0;

        foreach (Trabajador t in trabajadores)
        {
            bool esMOD = t.Clasificacion == ClasificacionLaboral.ManoDeObraDirecta;
            if (esMOD) { modINSSP += t.INSSPatronal; modINAT += t.INATEC; modVac += t.Vacaciones; modTrec += t.TreceavoMes; }
            else       { moiINSSP += t.INSSPatronal; moiINAT += t.INATEC; moiVac += t.Vacaciones; moiTrec += t.TreceavoMes; }
        }

        decimal totalPrest = SumaTotalPrestaciones();
        decimal cifPart    = moiINSSP + moiINAT + moiVac + moiTrec + modINSSP + modINAT + modVac + modTrec;

        // Todos los cargos de prestaciones van a CIF (igual que el Excel)
        var filas = new List<string[]>
        {
            new[] { "Control CIF / MOD (prestaciones MOD)", $"C$ {modINSSP+modINAT+modVac+modTrec:N2}",  "", "" },
            new[] { "Control CIF / MOI (prestaciones MOI)", $"C$ {moiINSSP+moiINAT+moiVac+moiTrec:N2}",  "", "" },
            new[] { "  INSS Patronal por pagar",            "",  $"C$ {SumaINSSPatronal():N2}", "" },
            new[] { "  INATEC por pagar",                   "",  $"C$ {SumaINATEC():N2}",        "" },
            new[] { "  Vacaciones por pagar",               "",  $"C$ {SumaVacaciones():N2}",    "" },
            new[] { "  Treceavo mes por pagar",             "",  $"C$ {SumaTreceavoMes():N2}",   "" }
        };

        TablaConsola.Imprimir(
            new[] { "Cuenta", "DEBE", "PARCIAL (Haber)", "HABER" },
            filas,
            alinDer: new[] { 1, 2, 3 }
        );

        Console.WriteLine($"  Comprobación DEBE = HABER: C$ {totalPrest:N2} = C$ {totalPrest:N2}");
    }

    // ── Tabla de tasas ────────────────────────────────────────────────────
    private void ImprimirTasas()
    {
        var filas = new List<string[]>
        {
            new[] { "INSS Laboral",  "7%  del Total Ingresos",         "Deducción al trabajador" },
            new[] { "IR Laboral",    "Tarifa progresiva anual (×12)",   "Deducción al trabajador" },
            new[] { "INSS Patronal", "22.5% del Total Ingresos",        "Cargo al empleador → CIF" },
            new[] { "INATEC",        "2%  del Total Ingresos",          "Cargo al empleador → CIF" },
            new[] { "Vacaciones",    "1/12 del Total Ingresos (8.33%)", "Cargo al empleador → CIF" },
            new[] { "Treceavo mes",  "1/12 del Total Ingresos (8.33%)", "Cargo al empleador → CIF" }
        };
        TablaConsola.Imprimir(
            new[] { "Concepto", "Tasa / Cálculo", "Naturaleza" },
            filas
        );
    }

    // ── Helper etiqueta clasificación ─────────────────────────────────────
    private static string ClasifLabel(ClasificacionLaboral c) => c switch
    {
        ClasificacionLaboral.ManoDeObraDirecta    => "MOD",
        ClasificacionLaboral.ManoDeObraIndirecta  => "MOI",
        ClasificacionLaboral.GastoDeVenta         => "G.Venta",
        ClasificacionLaboral.GastoAdministrativo  => "G.Admin",
        _                                          => c.ToString()
    };
}

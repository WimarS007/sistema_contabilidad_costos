class Trabajador
{
    // ── Datos básicos ─────────────────────────────────────────────────────
    private string nombre;
    private string noINSS;
    private string cargo;
    private string area;
    private ClasificacionLaboral clasificacion;

    // ── Horas ─────────────────────────────────────────────────────────────
    private decimal horasOrdinarias;
    private decimal horasExtras;

    // ── Salario base ──────────────────────────────────────────────────────
    /// <summary>Salario mensual ingresado por el usuario.</summary>
    private decimal salarioMensual;

    // ── Otros ingresos ────────────────────────────────────────────────────
    private decimal bono;
    private decimal antiguedad;          // Antigüedad (prestación adicional)

    // ─────────────────────────────────────────────────────────────────────
    // Tasas institucionales (constantes Nicaragua)
    // ─────────────────────────────────────────────────────────────────────
    public const decimal TASA_INSS_LABORAL   = 0.07m;   // 7%
    public const decimal TASA_INSS_PATRONAL  = 0.225m;  // 22.5%
    public const decimal TASA_INATEC         = 0.02m;   // 2%
    public const decimal TASA_VACACIONES     = 1m / 12m; // 8.33% mensual
    public const decimal TASA_TRECEAVO       = 1m / 12m; // 8.33% mensual

    // ─────────────────────────────────────────────────────────────────────
    // Tarifa progresiva del IR (Nicaragua – tramos anuales en C$)
    // ─────────────────────────────────────────────────────────────────────
    private static readonly (decimal Desde, decimal Hasta, decimal ImpBase, decimal Porcentaje, decimal SobreExceso)[] TRAMOS_IR =
    {
        (       1,  100_000,      0m, 0.00m,       0m),
        ( 100_001,  200_000,      0m, 0.15m, 100_000m),
        ( 200_001,  350_000, 15_000m, 0.20m, 200_000m),
        ( 350_001,  500_000, 45_000m, 0.25m, 350_000m),
        ( 500_001, decimal.MaxValue, 82_500m, 0.30m, 500_000m)
    };

    // ─────────────────────────────────────────────────────────────────────
    // Propiedades públicas con validación
    // ─────────────────────────────────────────────────────────────────────
    public string Nombre
    {
        get { return nombre; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El nombre del trabajador no puede quedar vacío.");
            else
                nombre = value;
        }
    }

    public string NoINSS
    {
        get { return noINSS; }
        set { noINSS = value ?? ""; }
    }

    public string Cargo
    {
        get { return cargo; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El cargo no puede quedar vacío.");
            else
                cargo = value;
        }
    }

    public string Area
    {
        get { return area; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El área no puede quedar vacía.");
            else
                area = value;
        }
    }

    public ClasificacionLaboral Clasificacion
    {
        get { return clasificacion; }
        set { clasificacion = value; }
    }

    public decimal HorasOrdinarias
    {
        get { return horasOrdinarias; }
        set
        {
            if (value < 0) Console.WriteLine("  Las horas ordinarias no pueden ser negativas.");
            else horasOrdinarias = value;
        }
    }

    public decimal HorasExtras
    {
        get { return horasExtras; }
        set
        {
            if (value < 0) Console.WriteLine("  Las horas extras no pueden ser negativas.");
            else horasExtras = value;
        }
    }

    public decimal SalarioMensual
    {
        get { return salarioMensual; }
        set
        {
            if (value < 0) Console.WriteLine("  El salario mensual no puede ser negativo.");
            else salarioMensual = value;
        }
    }

    public decimal Bono
    {
        get { return bono; }
        set
        {
            if (value < 0) Console.WriteLine("  El bono no puede ser negativo.");
            else bono = value;
        }
    }

    public decimal Antiguedad
    {
        get { return antiguedad; }
        set
        {
            if (value < 0) Console.WriteLine("  La antigüedad no puede ser negativa.");
            else antiguedad = value;
        }
    }

    // ─────────────────────────────────────────────────────────────────────
    // Propiedades calculadas — REMUNERACIÓN BRUTA
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>Tarifa hora extra = Salario mensual / 240 x 2</summary>
    public decimal TarifaHoraExtra
    {
        get { return (salarioMensual / 240m) * 2m; }
    }

    /// <summary>Ingreso por horas extras.</summary>
    public decimal IngresoHorasExtras
    {
        get { return horasExtras * TarifaHoraExtra; }
    }

    /// <summary>Total ingresos = Sal. mensual + Bono + Ing. HE + Antigüedad.</summary>
    public decimal TotalIngresos
    {
        get { return SalarioMensual + bono + IngresoHorasExtras + antiguedad; }
    }

    // ─────────────────────────────────────────────────────────────────────
    // DEDUCCIONES
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>INSS Laboral = 7% del Total Ingresos.</summary>
    public decimal INSSLaboral
    {
        get { return TotalIngresos * TASA_INSS_LABORAL; }
    }

    /// <summary>Base imponible para IR = Total Ingresos – INSS Laboral.</summary>
    public decimal BaseImponibleIR
    {
        get { return TotalIngresos - INSSLaboral; }
    }

    /// <summary>IR mensual calculado con tarifa progresiva anual.</summary>
    public decimal IRLaboral
    {
        get { return CalcularIRMensual(BaseImponibleIR); }
    }

    /// <summary>Total deducciones = INSS Laboral + IR Laboral.</summary>
    public decimal TotalDeducciones
    {
        get { return INSSLaboral + IRLaboral; }
    }

    // ─────────────────────────────────────────────────────────────────────
    // REMUNERACIÓN NETA
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>Neto a recibir = Total Ingresos – Total Deducciones.</summary>
    public decimal NetoARecibir
    {
        get { return TotalIngresos - TotalDeducciones; }
    }

    // ─────────────────────────────────────────────────────────────────────
    // PRESTACIONES SOCIALES (cargo al empleador)
    // ─────────────────────────────────────────────────────────────────────

    /// <summary>INSS Patronal = 22.5% del Total Ingresos.</summary>
    public decimal INSSPatronal
    {
        get { return TotalIngresos * TASA_INSS_PATRONAL; }
    }

    /// <summary>INATEC = 2% del Total Ingresos.</summary>
    public decimal INATEC
    {
        get { return TotalIngresos * TASA_INATEC; }
    }

    /// <summary>Vacaciones = Total Ingresos / 12.</summary>
    public decimal Vacaciones
    {
        get { return TotalIngresos * TASA_VACACIONES; }
    }

    /// <summary>Treceavo mes = Total Ingresos / 12.</summary>
    public decimal TreceavoMes
    {
        get { return TotalIngresos * TASA_TRECEAVO; }
    }

    /// <summary>Total prestaciones = INSS Patronal + INATEC + Vacaciones + Treceavo.</summary>
    public decimal TotalPrestaciones
    {
        get { return INSSPatronal + INATEC + Vacaciones + TreceavoMes; }
    }

    // ─────────────────────────────────────────────────────────────────────
    // Constructor
    // ─────────────────────────────────────────────────────────────────────
    public Trabajador()
    {
        nombre           = "";
        noINSS           = "";
        cargo            = "";
        area             = "";
        clasificacion    = ClasificacionLaboral.ManoDeObraDirecta;
        horasOrdinarias  = 0;
        horasExtras      = 0;
        salarioMensual   = 0;
        bono             = 0;
        antiguedad       = 0;
    }

    // ─────────────────────────────────────────────────────────────────────
    // Método privado: cálculo IR progresivo
    // ─────────────────────────────────────────────────────────────────────
    private static decimal CalcularIRMensual(decimal baseMensual)
    {
        decimal baseAnual = baseMensual * 12m;
        foreach (var (desde, hasta, impBase, pct, sobreExceso) in TRAMOS_IR)
        {
            if (baseAnual >= desde && baseAnual <= hasta)
            {
                if (pct == 0m) return 0m;
                decimal irAnual = impBase + (baseAnual - sobreExceso) * pct;
                return irAnual / 12m;
            }
        }
        return 0m;
    }
}

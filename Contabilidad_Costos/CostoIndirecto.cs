class CostoIndirecto
{
    private string concepto;
    private TipoCIF tipo;
    private ComportamientoCosto comportamiento;
    private decimal monto;
    private string areaRelacionada;
    private string observacion;

    public string Concepto
    {
        get { return concepto; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El concepto del CIF no puede quedar vacío.");
            else
                concepto = value;
        }
    }

    public TipoCIF Tipo
    {
        get { return tipo; }
        set { tipo = value; }
    }

    public ComportamientoCosto Comportamiento
    {
        get { return comportamiento; }
        set { comportamiento = value; }
    }

    public decimal Monto
    {
        get { return monto; }
        set
        {
            if (value < 0)
                Console.WriteLine("  El monto del CIF no puede ser negativo.");
            else
                monto = value;
        }
    }

    public string AreaRelacionada
    {
        get { return areaRelacionada; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El área relacionada no puede quedar vacía.");
            else
                areaRelacionada = value;
        }
    }

    public string Observacion
    {
        get { return observacion; }
        set { observacion = value ?? ""; }
    }

    public CostoIndirecto()
    {
        concepto        = "";
        areaRelacionada = "";
        observacion     = "";
        tipo            = TipoCIF.OtroCIF;
        comportamiento  = ComportamientoCosto.Fijo;
        monto           = 0;
    }
}

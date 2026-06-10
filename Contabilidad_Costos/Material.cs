class Material
{
    private string codigo;
    private string nombre;
    private TipoMaterial tipo;
    private string unidadMedida;
    private decimal cantidadDisponible;
    private decimal cantidadConsumida;
    private decimal costoUnitario;

    public string Codigo
    {
        get { return codigo; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El código del material no puede quedar vacío.");
            else
                codigo = value;
        }
    }

    public string Nombre
    {
        get { return nombre; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  El nombre del material no puede quedar vacío.");
            else
                nombre = value;
        }
    }

    public TipoMaterial Tipo
    {
        get { return tipo; }
        set { tipo = value; }
    }

    public string UnidadMedida
    {
        get { return unidadMedida; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                Console.WriteLine("  La unidad de medida no puede quedar vacía.");
            else
                unidadMedida = value;
        }
    }

    public decimal CantidadDisponible
    {
        get { return cantidadDisponible; }
        set
        {
            if (value < 0)
                Console.WriteLine("  La cantidad disponible no puede ser negativa.");
            else
                cantidadDisponible = value;
        }
    }

    public decimal CantidadConsumida
    {
        get { return cantidadConsumida; }
        set
        {
            if (value < 0)
                Console.WriteLine("  La cantidad consumida no puede ser negativa.");
            else if (value > cantidadDisponible)
                Console.WriteLine("  La cantidad consumida no puede superar la disponible.");
            else
                cantidadConsumida = value;
        }
    }

    public decimal CostoUnitario
    {
        get { return costoUnitario; }
        set
        {
            if (value < 0)
                Console.WriteLine("  El costo unitario no puede ser negativo.");
            else
                costoUnitario = value;
        }
    }

    public decimal CostoTotal
    {
        get { return cantidadConsumida * costoUnitario; }
    }

    public Material()
    {
        codigo             = "";
        nombre             = "";
        unidadMedida       = "";
        tipo               = TipoMaterial.Directo;
        cantidadDisponible = 0;
        cantidadConsumida  = 0;
        costoUnitario      = 0;
    }
}

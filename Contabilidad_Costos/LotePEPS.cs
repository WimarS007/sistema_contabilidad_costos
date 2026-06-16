/// <summary>
/// Representa un lote de inventario en la cola PEPS (Primero en Entrar, Primero en Salir).
/// Cada compra o entrada genera un lote independiente con su costo unitario propio.
/// </summary>
class LotePEPS
{
    public decimal Cantidad      { get; set; }
    public decimal CostoUnitario { get; }

    public decimal ValorTotal => Cantidad * CostoUnitario;

    public LotePEPS(decimal cantidad, decimal costoUnitario)
    {
        Cantidad      = cantidad;
        CostoUnitario = costoUnitario;
    }
}

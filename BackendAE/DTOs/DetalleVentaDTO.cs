namespace BackendAE.DTOs
{
    public class DetalleVentaDTO
    {
        public int DetalleVentaId { get; set; }

        public int ProductoId { get; set; }

        public string? NombreProducto { get; set; } // Lo puedes llenar desde Producto.Nombre

        public int Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal { get; set; }
    }
}

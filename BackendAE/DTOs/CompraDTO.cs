using BackendAE.Models;

namespace BackendAE.DTOs
{
    public class CompraDTO
    {
        public int CompraId { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }

        public Proveedor Proveedor { get; set; }
        public UsuarioSimpleDTO Usuario { get; set; } // Usar el DTO simplificado
        public List<DetalleCompraDTO> DetallesCompra { get; set; }
    }

    public class DetalleCompraDTO
    {
        public int DetalleCompraId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public Producto Producto { get; set; }
    }

}
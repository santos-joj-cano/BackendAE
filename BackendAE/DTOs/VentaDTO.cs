using BackendAE.Models;

namespace BackendAE.DTOs
{
    public class VentaDTO
    {
        public int VentaId { get; set; }
        public string CodigoVenta { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal Total { get; set; }
        public decimal EfectivoRecibido { get; set; }
        public decimal Cambio { get; set; }
        public string EstadoVenta { get; set; }

        public UsuarioSimpleDTO Usuario { get; set; }
        public CajaSesionDTO CajaSesion { get; set; }
        public List<DetalleVentaDTO> DetallesVenta { get; set; }
    }

    public class DetalleVentaDTO
    {
        public int DetalleVentaId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        // Se puede añadir un DTO para el producto si se necesita más información
        public Producto Producto { get; set; }
    }
}
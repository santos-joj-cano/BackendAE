namespace BackendAE.DTOs
{
    public class VentaRequestDTO
    {
        public int UsuarioId { get; set; }
        public int? CajaSesionId { get; set; }
        public decimal EfectivoRecibido { get; set; }
        public string EstadoVenta { get; set; }
        public List<DetalleVentaRequestDTO> DetallesVenta { get; set; }
    }

    public class DetalleVentaRequestDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
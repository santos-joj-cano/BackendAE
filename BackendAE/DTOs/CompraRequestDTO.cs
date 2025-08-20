namespace BackendAE.DTOs
{
    public class CompraRequestDTO
    {
        public int ProveedorId { get; set; }
        public int UsuarioId { get; set; }
        public List<DetalleCompraRequestDTO> DetallesCompra { get; set; }
    }

    public class DetalleCompraRequestDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
    }
}
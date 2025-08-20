namespace BackendAE.DTOs
{
    public class ProductoRequestDTO
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioAdquisicion { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public int CategoriaProductoId { get; set; }
        public int ProveedorId { get; set; }
    }
}
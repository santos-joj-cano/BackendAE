using BackendAE.Models;

namespace BackendAE.DTOs
{
    public class ProductoDTO
    {
        public int ProductoId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioAdquisicion { get; set; }
        public decimal PrecioVenta { get; set; }
        public int Stock { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Estado { get; set; }
        public CategoriaProducto CategoriaProducto { get; set; }
        public Proveedor Proveedor { get; set; }
    }
}
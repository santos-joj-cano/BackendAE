using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class DetalleVentaCreacionDTO
    {
        [Required]
        public required int ProductoId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero.")]
        public required int Cantidad { get; set; }
    }
}
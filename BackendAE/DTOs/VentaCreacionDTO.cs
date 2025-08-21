using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class VentaCreacionDTO
    {
        [Required]
        public required int UsuarioId { get; set; }

        public int? CajaSesionId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "La venta debe contener al menos un producto.")]
        public required List<DetalleVentaCreacionDTO> DetalleVentas { get; set; }

        [Required]
        public required decimal EfectivoRecibido { get; set; }
    }
}
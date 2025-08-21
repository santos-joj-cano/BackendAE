using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class MovimientoCajaCreacionDTO
    {
        [Required]
        [StringLength(12)]
        public required string Tipo { get; set; }

        [Required]
        [StringLength(120)]
        public required string Concepto { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto no puede ser un valor negativo.")]
        public required decimal Monto { get; set; }

        [Required]
        public required int CajaSesionId { get; set; }

        [Required]
        public required int UsuarioId { get; set; }
    }
}
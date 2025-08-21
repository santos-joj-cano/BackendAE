using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class CajaSesionCreacionDTO
    {
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "El monto de apertura no puede ser un valor negativo.")]
        public required decimal MontoApertura { get; set; }

        public string? Observacion { get; set; }

        [Required]
        public required int CajaId { get; set; }

        [Required]
        public required int UsuarioAperturaId { get; set; }
    }
}
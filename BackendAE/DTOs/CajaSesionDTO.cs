using BackendAE.Models;

namespace BackendAE.DTOs
{
    public class CajaSesionDTO
    {
        public int CajaSesionId { get; set; }
        public string CodigoSesion { get; set; }
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal MontoInicial { get; set; }
        public decimal TotalVentas { get; set; }
        public decimal MontoCierre { get; set; }
        public string Estado { get; set; }
        public UsuarioSimpleDTO Usuario { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAE.Models
{
    public class Compra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CompraId { get; set; }

        [Required]
        public required DateTime FechaCompra { get; set; }

        [Required]
        [Column(TypeName = "decimal(12, 2)")]
        public required decimal Total { get; set; }

        [StringLength(250)]
        public string? Observacion { get; set; }

        // Clave foránea N:1 con Proveedor
        [ForeignKey("Proveedor")]
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

        // Relación 1:N
        public ICollection<DetalleCompra>? DetallesCompras { get; set; }
    }
}
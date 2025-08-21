using System.ComponentModel.DataAnnotations;

namespace BackendAE.DTOs
{
    public class UsuarioCreacionDTO
    {
        [Required(ErrorMessage = "El primer nombre es obligatorio.")]
        [StringLength(60, ErrorMessage = "El primer nombre no puede exceder los 60 caracteres.")]
        public required string PrimerNombre { get; set; }

        [StringLength(60, ErrorMessage = "El segundo nombre no puede exceder los 60 caracteres.")]
        public string? SegundoNombre { get; set; }

        [Required(ErrorMessage = "El primer apellido es obligatorio.")]
        [StringLength(60, ErrorMessage = "El primer apellido no puede exceder los 60 caracteres.")]
        public required string PrimerApellido { get; set; }

        [StringLength(60, ErrorMessage = "El segundo apellido no puede exceder los 60 caracteres.")]
        public string? SegundoApellido { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(120, ErrorMessage = "El email no puede exceder los 120 caracteres.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public required string Contrasena { get; set; }

        public int RolId { get; set; }

        public bool Estado { get; set; } = true;

        [StringLength(20, ErrorMessage = "El NIT no puede exceder los 20 caracteres.")]
        public string? NIT { get; set; }

        [StringLength(25, ErrorMessage = "El CUI no puede exceder los 25 caracteres.")]
        public string? CUI { get; set; }
    }
}
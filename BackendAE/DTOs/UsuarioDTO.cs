namespace BackendAE.DTOs
{
    public class UsuarioDTO
    {
        public int UsuarioId { get; set; }
        public required string PrimerNombre { get; set; }
        public required string SegundoNombre { get; set; }
        public required string PrimerApellido { get; set; }
        public required string SegundoApellido { get; set; }
        public required string NombreUsuario { get; set; }
        public required string Email { get; set; }
        public required string RolNombre { get; set; } // Propiedad para el nombre del rol
        public bool Estado { get; set; }
        // Se pueden agregar más campos según sea necesario
    }
}
namespace BackendAE.DTOs
{
    public class UsuarioRequestDTO
    {
        public required string PrimerNombre { get; set; }
        public required string SegundoNombre { get; set; }
        public required string PrimerApellido { get; set; }
        public required string SegundoApellido { get; set; }
        public required string NombreUsuario { get; set; } // Asegúrate de que este sea el nombre
        public required string Password { get; set; }
        public required int RolId { get; set; }
        public required string Email { get; set; }
    }
}
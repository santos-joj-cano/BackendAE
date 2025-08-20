using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendAE.Services;
using BCrypt.Net;
using System.Security.Cryptography;
using BackendAE.DTOs; // Importa los DTOs
using Microsoft.AspNetCore.Authorization; // Para el atributo [Authorize]

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public UsuariosController(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Select(u => new UsuarioDTO
                {
                    UsuarioId = u.UsuarioId,
                    PrimerNombre = u.PrimerNombre,
                    SegundoNombre = u.SegundoNombre,
                    PrimerApellido = u.PrimerApellido,
                    SegundoApellido = u.SegundoApellido,
                    NombreUsuario = u.NombreUsuario, // Asegúrate de usar NombreUsuario
                    Email = u.Email,
                    Estado = u.Estado,
                    RolNombre = u.Rol.RolNombre
                })
                .ToListAsync();

            return usuarios;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
            {
                return NotFound();
            }

            var usuarioDTO = new UsuarioDTO
            {
                UsuarioId = usuario.UsuarioId,
                PrimerNombre = usuario.PrimerNombre,
                SegundoNombre = usuario.SegundoNombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                NombreUsuario = usuario.NombreUsuario, // Asegúrate de usar NombreUsuario
                Email = usuario.Email,
                Estado = usuario.Estado,
                RolNombre = usuario.Rol.RolNombre
            };

            return usuarioDTO;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutUsuario(int id, UsuarioRequestDTO usuarioRequest)
        {
            var usuarioExistente = await _context.Usuarios.FindAsync(id);
            if (usuarioExistente == null)
            {
                return NotFound();
            }

            usuarioExistente.PrimerNombre = usuarioRequest.PrimerNombre;
            usuarioExistente.SegundoNombre = usuarioRequest.SegundoNombre;
            usuarioExistente.PrimerApellido = usuarioRequest.PrimerApellido;
            usuarioExistente.SegundoApellido = usuarioRequest.SegundoApellido;
            usuarioExistente.NombreUsuario = usuarioRequest.NombreUsuario;
            usuarioExistente.Email = usuarioRequest.Email;
            usuarioExistente.RolId = usuarioRequest.RolId;

            if (!string.IsNullOrEmpty(usuarioRequest.Password))
            {
                usuarioExistente.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuarioRequest.Password);
            }

            _context.Entry(usuarioExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UsuarioDTO>> PostUsuario(UsuarioRequestDTO usuarioRequest)
        {
            // Generar la contraseña temporal ANTES de crear el objeto
            var temporaryPassword = GenerateTemporaryPassword();

            // Hashear la contraseña temporal
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);

            // Crear una instancia del modelo de base de datos a partir del DTO
            var usuario = new Usuario
            {
                PrimerNombre = usuarioRequest.PrimerNombre,
                SegundoNombre = usuarioRequest.SegundoNombre,
                PrimerApellido = usuarioRequest.PrimerApellido,
                SegundoApellido = usuarioRequest.SegundoApellido,
                NombreUsuario = usuarioRequest.NombreUsuario,
                Email = usuarioRequest.Email,
                Estado = true,
                RolId = usuarioRequest.RolId,
                PasswordHash = hashedPassword,
                FechaIngreso = DateTime.Now // Asignar la fecha de ingreso
            };

            // Mensaje del correo electrónico
            var emailBody = $"Hola {usuario.PrimerNombre},<br><br>" +
                            $"Tu cuenta ha sido creada. Aquí están tus credenciales temporales:<br><br>" +
                            $"<b>Nombre de usuario:</b> {usuario.NombreUsuario}<br>" +
                            $"<b>Contraseña:</b> {temporaryPassword}<br><br>" +
                            "Por favor, cambia tu contraseña en tu primer inicio de sesión.<br><br>" +
                            "Saludos,<br>Equipo de BackendAE";

            // Enviar el correo electrónico
            await _emailService.SendEmailAsync(usuario.Email, "Tu cuenta ha sido creada", emailBody);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var rol = await _context.Roles.FindAsync(usuario.RolId);

            var usuarioDTO = new UsuarioDTO
            {
                UsuarioId = usuario.UsuarioId,
                PrimerNombre = usuario.PrimerNombre,
                SegundoNombre = usuario.SegundoNombre,
                PrimerApellido = usuario.PrimerApellido,
                SegundoApellido = usuario.SegundoApellido,
                NombreUsuario = usuario.NombreUsuario,
                Email = usuario.Email,
                Estado = usuario.Estado,
                RolNombre = rol?.RolNombre
            };

            return CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuarioDTO);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }

        private string GenerateTemporaryPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            using var rng = RandomNumberGenerator.Create();
            var passwordBytes = new byte[12];
            rng.GetBytes(passwordBytes);

            var password = new char[12];
            for (int i = 0; i < passwordBytes.Length; i++)
            {
                password[i] = validChars[passwordBytes[i] % validChars.Length];
            }

            return new string(password);
        }
    }
}
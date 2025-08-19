using BackendAE.Data;
using BackendAE.Models;
using BackendAE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using System.Security.Cryptography;

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

        // GET: api/Usuarios
        [HttpGet]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios
                .Include(u => u.Rol) // Incluye la información del rol
                .ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.UsuarioId)
            {
                return BadRequest();
            }

            // Ahora se tiene un apartado PUT específico para la actualización de la contraseña.
            _context.Entry(usuario).State = EntityState.Modified;
            _context.Entry(usuario).Property(x => x.PasswordHash).IsModified = false;

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

            // Obtener el usuario de la base de datos
            var existingUser = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (existingUser == null)
            {
                return NotFound();
            }

            // Asignar el PasswordHash existente al objeto que se va a actualizar
            usuario.PasswordHash = existingUser.PasswordHash;

            _context.Entry(usuario).State = EntityState.Modified;

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

        // POST: api/Usuarios
        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // Generar una contraseña temporal aleatoria
            var temporaryPassword = GenerateTemporaryPassword();

            // Mensaje de advertencia: enviar la contraseña por correo no es una práctica segura.
            // Es preferible que el usuario establezca su propia contraseña al iniciar sesión por primera vez.
            var emailBody = $"Hola {usuario.PrimerNombre},<br><br>" +
                            $"Tu cuenta ha sido creada. Aquí están tus credenciales temporales:<br><br>" +
                            $"<b>Nombre de usuario:</b> {usuario.NombreUsuario}<br>" +
                            $"<b>Contraseña:</b> {temporaryPassword}<br><br>" +
                            "Por favor, cambia tu contraseña en tu primer inicio de sesión.<br><br>" +
                            "Saludos,<br>Equipo de BackendAE";

            // Enviar el correo electrónico
            await _emailService.SendEmailAsync(usuario.Email, "Tu cuenta ha sido creada", emailBody);

            // Hashear la contraseña temporal antes de guardarla en la base de datos
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.UsuarioId }, usuario);
        }

        // Método para generar una contraseña aleatoria
        private string GenerateTemporaryPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            using var rng = RandomNumberGenerator.Create();
            var passwordBytes = new byte[12]; // 12 caracteres de longitud
            rng.GetBytes(passwordBytes);

            var password = new char[12];
            for (int i = 0; i < passwordBytes.Length; i++)
            {
                password[i] = validChars[passwordBytes[i] % validChars.Length];
            }

            return new string(password);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
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

        // PUT: api/Usuarios/UpdatePassword/5
        [HttpPut("UpdatePassword/{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Solo acceso para Admin
        public async Task<IActionResult> PutPassword(int id, [FromBody] string newPassword)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound("Usuario no encontrado.");
            }

            if (string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("La nueva contraseña no puede ser nula o vacía.");
            }

            // Hashear la nueva contraseña antes de guardarla
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

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

        // Método auxiliar para verificar si un usuario existe
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.UsuarioId == id);
        }
    }
}
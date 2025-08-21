using AutoMapper;
using BackendAE.Data;
using BackendAE.DTOs;
using BackendAE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;

namespace BackendAE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsuariosController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .ToListAsync();

            var usuariosDTO = _mapper.Map<List<UsuarioDTO>>(usuarios);
            return Ok(usuariosDTO);
        }

        // GET: api/Usuarios/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.UsuarioId == id);

            if (usuario == null) return NotFound();

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult> CrearUsuario([FromBody] UsuarioCreacionDTO dto)
        {
            var usuario = _mapper.Map<Usuario>(dto);
            usuario.PasswordHash = dto.Contrasena; // Aquí deberías aplicar hashing

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.UsuarioId }, usuarioDTO);
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> ActualizarUsuario(int id, [FromBody] UsuarioCreacionDTO dto)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _mapper.Map(dto, usuario);
            usuario.PasswordHash = dto.Contrasena; // Aquí también debería aplicarse hashing

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

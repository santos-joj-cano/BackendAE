using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendAE.DTOs;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajaSesionesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CajaSesionesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CajaSesion
        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")]
        public async Task<ActionResult<IEnumerable<CajaSesionDTO>>> GetCajaSesiones()
        {
            var sesiones = await _context.CajaSesiones
                .Include(cs => cs.Usuario)
                .ToListAsync();

            var sesionesDTO = sesiones.Select(cs => new CajaSesionDTO
            {
                CajaSesionId = cs.CajaSesionId,
                CodigoSesion = cs.CodigoSesion,
                FechaApertura = cs.FechaApertura,
                FechaCierre = cs.FechaCierre,
                MontoInicial = cs.MontoInicial,
                TotalVentas = cs.TotalVentas,
                MontoCierre = cs.MontoCierre,
                Estado = cs.Estado,
                Usuario = new UsuarioSimpleDTO
                {
                    UsuarioId = cs.Usuario.UsuarioId,
                    NombreCompleto = $"{cs.Usuario.PrimerNombre} {cs.Usuario.PrimerApellido}",
                    NombreUsuario = cs.Usuario.NombreUsuario
                }
            }).ToList();

            return sesionesDTO;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<CajaSesion>> GetCajaSesion(int id)
        {
            var cajaSesion = await _context.CajaSesiones
                .Include(cs => cs.Caja)
                .Include(cs => cs.UsuarioApertura)
                .Include(cs => cs.UsuarioCierre)
                .FirstOrDefaultAsync(cs => cs.CajaSesionId == id);
            if (cajaSesion == null)
            {
                return NotFound();
            }
            return cajaSesion;
        }

        


        // PUT: api/CajaSesiones/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Access para Admin y Empleado
        public async Task<IActionResult> PutCajaSesion(int id, CajaSesion cajaSesion)
        {
            if (id != cajaSesion.CajaSesionId)
            {
                return BadRequest();
            }
            _context.Entry(cajaSesion).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CajaSesionExists(id))
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

        // POST: api/CajaSesiones
        [HttpPost]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<CajaSesion>> PostCajaSesion(CajaSesion cajaSesion)
        {
            _context.CajaSesiones.Add(cajaSesion);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCajaSesion", new { id = cajaSesion.CajaSesionId }, cajaSesion);
        }

        // DELETE: api/CajaSesiones/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> DeleteCajaSesion(int id)
        {
            var cajaSesion = await _context.CajaSesiones.FindAsync(id);
            if (cajaSesion == null)
            {
                return NotFound();
            }
            _context.CajaSesiones.Remove(cajaSesion);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CajaSesionExists(int id)
        {
            return _context.CajaSesiones.Any(e => e.CajaSesionId == id);
        }
    }
}
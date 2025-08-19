using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class MovimientosCajaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovimientosCajaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<IEnumerable<MovimientoCaja>>> GetMovimientosCaja()
        {
            return await _context.MovimientosCaja
                .Include(mc => mc.CajaSesion)
                .Include(mc => mc.Usuario)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<MovimientoCaja>> GetMovimientoCaja(int id)
        {
            var movimientoCaja = await _context.MovimientosCaja
                .Include(mc => mc.CajaSesion)
                .Include(mc => mc.Usuario)
                .FirstOrDefaultAsync(mc => mc.MovimientoCajaId == id);
            if (movimientoCaja == null)
            {
                return NotFound();
            }
            return movimientoCaja;
        }

        // PUT: api/MovimientosCaja/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Access para Admin y Empleado
        public async Task<IActionResult> PutMovimientoCaja(int id, MovimientoCaja movimientoCaja)
        {
            if (id != movimientoCaja.MovimientoCajaId)
            {
                return BadRequest();
            }
            _context.Entry(movimientoCaja).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovimientoCajaExists(id))
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

        // POST: api/MovimientosCaja
        [HttpPost]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<MovimientoCaja>> PostMovimientoCaja(MovimientoCaja movimientoCaja)
        {
            _context.MovimientosCaja.Add(movimientoCaja);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetMovimientoCaja", new { id = movimientoCaja.MovimientoCajaId }, movimientoCaja);
        }

        // DELETE: api/MovimientosCaja/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> DeleteMovimientoCaja(int id)
        {
            var movimientoCaja = await _context.MovimientosCaja.FindAsync(id);
            if (movimientoCaja == null)
            {
                return NotFound();
            }
            _context.MovimientosCaja.Remove(movimientoCaja);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool MovimientoCajaExists(int id)
        {
            return _context.MovimientosCaja.Any(e => e.MovimientoCajaId == id);
        }
    }
}
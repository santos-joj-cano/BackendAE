using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CajasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CajasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")] // Access para Admin y Empleado
        public async Task<ActionResult<IEnumerable<Caja>>> GetCajas()
        {
            return await _context.Cajas.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")] // Access para Admin y Empleado
        public async Task<ActionResult<Caja>> GetCaja(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);
            if (caja == null)
            {
                return NotFound();
            }
            return caja;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Access para Admin y Empleado
        public async Task<IActionResult> PutCaja(int id, Caja caja)
        {
            if (id != caja.CajaId)
            {
                return BadRequest();
            }
            _context.Entry(caja).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CajaExists(id))
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
        [Authorize(Roles = "Admin")] // Access para Admin y Empleado
        public async Task<ActionResult<Caja>> PostCaja(Caja caja)
        {
            _context.Cajas.Add(caja);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCaja", new { id = caja.CajaId }, caja);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> DeleteCaja(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);
            if (caja == null)
            {
                return NotFound();
            }
            _context.Cajas.Remove(caja);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CajaExists(int id)
        {
            return _context.Cajas.Any(e => e.CajaId == id);
        }
    }
}
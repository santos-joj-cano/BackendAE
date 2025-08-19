using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    
    [ApiController]
    public class DetalleComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DetalleComprasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<IEnumerable<DetalleCompra>>> GetDetalleCompras()
        {
            return await _context.DetallesCompras
                .Include(dc => dc.Compra)
                .Include(dc => dc.Producto)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<DetalleCompra>> GetDetalleCompra(int id)
        {
            var detalleCompra = await _context.DetallesCompras
                .Include(dc => dc.Compra)
                .Include(dc => dc.Producto)
                .FirstOrDefaultAsync(dc => dc.DetalleCompraId == id);
            if (detalleCompra == null)
            {
                return NotFound();
            }
            return detalleCompra;
        }

        // PUT: api/DetalleCompras/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Access para Admin y Empleado
        public async Task<IActionResult> PutDetalleCompra(int id, DetalleCompra detalleCompra)
        {
            if (id != detalleCompra.DetalleCompraId)
            {
                return BadRequest();
            }
            _context.Entry(detalleCompra).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetalleCompraExists(id))
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

        // POST: api/DetalleCompras
        [HttpPost]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<DetalleCompra>> PostDetalleCompra(DetalleCompra detalleCompra)
        {
            _context.DetallesCompras.Add(detalleCompra);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetDetalleCompra", new { id = detalleCompra.DetalleCompraId }, detalleCompra);
        }

        // DELETE: api/DetalleCompras/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> DeleteDetalleCompra(int id)
        {
            var detalleCompra = await _context.DetallesCompras.FindAsync(id);
            if (detalleCompra == null)
            {
                return NotFound();
            }
            _context.DetallesCompras.Remove(detalleCompra);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool DetalleCompraExists(int id)
        {
            return _context.DetallesCompras.Any(e => e.DetalleCompraId == id);
        }
    }
}
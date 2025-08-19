using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
   
    [ApiController]
    public class DetalleVentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DetalleVentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<IEnumerable<DetalleVenta>>> GetDetalleVentas()
        {
            return await _context.DetallesVentas
                .Include(dv => dv.Venta)
                .Include(dv => dv.Producto)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<DetalleVenta>> GetDetalleVenta(int id)
        {
            var detalleVenta = await _context.DetallesVentas
                .Include(dv => dv.Venta)
                .Include(dv => dv.Producto)
                .FirstOrDefaultAsync(dv => dv.DetalleVentaId == id);
            if (detalleVenta == null)
            {
                return NotFound();
            }
            return detalleVenta;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<IActionResult> PutDetalleVenta(int id, DetalleVenta detalleVenta)
        {
            if (id != detalleVenta.DetalleVentaId)
            {
                return BadRequest();
            }
            _context.Entry(detalleVenta).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DetalleVentaExists(id))
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

        // POST: api/DetalleVentas
        [HttpPost]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<DetalleVenta>> PostDetalleVenta(DetalleVenta detalleVenta)
        {
            _context.DetallesVentas.Add(detalleVenta);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetDetalleVenta", new { id = detalleVenta.DetalleVentaId }, detalleVenta);
        }

        // Delete: api/DetalleVentas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> DeleteDetalleVenta(int id)
        {
            var detalleVenta = await _context.DetallesVentas.FindAsync(id);
            if (detalleVenta == null)
            {
                return NotFound();
            }
            _context.DetallesVentas.Remove(detalleVenta);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool DetalleVentaExists(int id)
        {
            return _context.DetallesVentas.Any(e => e.DetalleVentaId == id);
        }
    }
}
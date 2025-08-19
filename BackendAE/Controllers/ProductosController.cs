using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.Include(p => p.CategoriaProducto).ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .FirstOrDefaultAsync(p => p.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }
            return producto;
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> PutProducto(int id, Producto producto)
        {
            if (id != producto.ProductoId)
            {
                return BadRequest();
            }
            _context.Entry(producto).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductoExists(id))
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

        // POST: api/Productos
        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProducto", new { id = producto.ProductoId }, producto);
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}
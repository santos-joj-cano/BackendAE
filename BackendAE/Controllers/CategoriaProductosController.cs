using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriaProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/CategoriaProductos
        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<IEnumerable<CategoriaProducto>>> GetCategoriaProductos()
        {
            return await _context.CategoriasProductos.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<CategoriaProducto>> GetCategoriaProducto(int id)
        {
            var categoriaProducto = await _context.CategoriasProductos.FindAsync(id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }
            return categoriaProducto;
        }

        // PUT: api/CategoriaProductos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> PutCategoriaProducto(int id, CategoriaProducto categoriaProducto)
        {
            if (id != categoriaProducto.CategoriaId)
            {
                return BadRequest();
            }
            _context.Entry(categoriaProducto).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaProductoExists(id))
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

        // POST: api/CategoriaProductos
        [HttpPost]
        [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<CategoriaProducto>> PostCategoriaProducto(CategoriaProducto categoriaProducto)
        {
            _context.CategoriasProductos.Add(categoriaProducto);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCategoriaProducto", new { id = categoriaProducto.CategoriaId }, categoriaProducto);
        }

        // DELETE: api/CategoriaProductos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategoriaProducto(int id)
        {
            var categoriaProducto = await _context.CategoriasProductos.FindAsync(id);
            if (categoriaProducto == null)
            {
                return NotFound();
            }
            _context.CategoriasProductos.Remove(categoriaProducto);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CategoriaProductoExists(int id)
        {
            return _context.CategoriasProductos.Any(e => e.CategoriaId == id);
        }
    }
}
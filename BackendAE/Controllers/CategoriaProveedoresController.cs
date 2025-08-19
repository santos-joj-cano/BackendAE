using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriaProveedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<IEnumerable<CategoriaProveedor>>> GetCategoriaProveedores()
        {
            return await _context.CategoriasProveedores.ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<CategoriaProveedor>> GetCategoriaProveedor(int id)
        {
            var categoriaProveedor = await _context.CategoriasProveedores.FindAsync(id);
            if (categoriaProveedor == null)
            {
                return NotFound();
            }
            return categoriaProveedor;
        }

        // PUT: api/CategoriaProveedores/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> PutCategoriaProveedor(int id, CategoriaProveedor categoriaProveedor)
        {
            if (id != categoriaProveedor.CatProveedorId)
            {
                return BadRequest();
            }
            _context.Entry(categoriaProveedor).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoriaProveedorExists(id))
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

        // POST: api/CategoriaProveedores
        [HttpPost]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<ActionResult<CategoriaProveedor>> PostCategoriaProveedor(CategoriaProveedor categoriaProveedor)
        {
            _context.CategoriasProveedores.Add(categoriaProveedor);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCategoriaProveedor", new { id = categoriaProveedor.CatProveedorId }, categoriaProveedor);
        }

        // DELETE: api/CategoriaProveedores/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> DeleteCategoriaProveedor(int id)
        {
            var categoriaProveedor = await _context.CategoriasProveedores.FindAsync(id);
            if (categoriaProveedor == null)
            {
                return NotFound();
            }
            _context.CategoriasProveedores.Remove(categoriaProveedor);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool CategoriaProveedorExists(int id)
        {
            return _context.CategoriasProveedores.Any(e => e.CatProveedorId == id);
        }
    }
}
using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using BackendAE.DTOs;

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

        // GET: api/Productos
        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductos()
        {
            var productos = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .Include(p => p.Proveedor)
                .Where(p => p.Estado)
                .ToListAsync();

            var productosDTO = productos.Select(p => new ProductoDTO
            {
                ProductoId = p.ProductoId,
                Codigo = p.Codigo,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                PrecioAdquisicion = p.PrecioAdquisicion,
                PrecioVenta = p.PrecioVenta,
                Stock = p.Stock,
                FechaRegistro = p.FechaRegistro,
                Estado = p.Estado,
                CategoriaProducto = p.CategoriaProducto,
                Proveedor = p.Proveedor
            }).ToList();

            return productosDTO;
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.ProductoId == id);

            if (producto == null)
            {
                return NotFound();
            }

            var productoDTO = new ProductoDTO
            {
                ProductoId = producto.ProductoId,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                PrecioAdquisicion = producto.PrecioAdquisicion,
                PrecioVenta = producto.PrecioVenta,
                Stock = producto.Stock,
                FechaRegistro = producto.FechaRegistro,
                Estado = producto.Estado,
                CategoriaProducto = producto.CategoriaProducto,
                Proveedor = producto.Proveedor
            };

            return productoDTO;
        }

        // POST: api/Productos
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductoDTO>> PostProducto(ProductoRequestDTO productoRequest)
        {
            var producto = new Producto
            {
                Codigo = productoRequest.Codigo,
                Nombre = productoRequest.Nombre,
                Descripcion = productoRequest.Descripcion,
                PrecioAdquisicion = productoRequest.PrecioAdquisicion,
                PrecioVenta = productoRequest.PrecioVenta,
                Stock = productoRequest.Stock,
                CategoriaProductoId = productoRequest.CategoriaProductoId,
                ProveedorId = productoRequest.ProveedorId,
                FechaRegistro = DateTime.Now,
                Estado = true
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            // Mapear y devolver el DTO de respuesta con las relaciones cargadas
            var productoGuardado = await _context.Productos
                .Include(p => p.CategoriaProducto)
                .Include(p => p.Proveedor)
                .FirstOrDefaultAsync(p => p.ProductoId == producto.ProductoId);

            var productoDTO = new ProductoDTO
            {
                ProductoId = productoGuardado.ProductoId,
                Codigo = productoGuardado.Codigo,
                Nombre = productoGuardado.Nombre,
                Descripcion = productoGuardado.Descripcion,
                PrecioAdquisicion = productoGuardado.PrecioAdquisicion,
                PrecioVenta = productoGuardado.PrecioVenta,
                Stock = productoGuardado.Stock,
                FechaRegistro = productoGuardado.FechaRegistro,
                Estado = productoGuardado.Estado,
                CategoriaProducto = productoGuardado.CategoriaProducto,
                Proveedor = productoGuardado.Proveedor
            };

            return CreatedAtAction("GetProducto", new { id = producto.ProductoId }, productoDTO);
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutProducto(int id, ProductoRequestDTO productoRequest)
        {
            var productoExistente = await _context.Productos.FindAsync(id);
            if (productoExistente == null)
            {
                return NotFound();
            }

            productoExistente.Codigo = productoRequest.Codigo;
            productoExistente.Nombre = productoRequest.Nombre;
            productoExistente.Descripcion = productoRequest.Descripcion;
            productoExistente.PrecioAdquisicion = productoRequest.PrecioAdquisicion;
            productoExistente.PrecioVenta = productoRequest.PrecioVenta;
            productoExistente.Stock = productoRequest.Stock;
            productoExistente.CategoriaProductoId = productoRequest.CategoriaProductoId;
            productoExistente.ProveedorId = productoRequest.ProveedorId;

            _context.Entry(productoExistente).State = EntityState.Modified;

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

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            producto.Estado = false;
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}
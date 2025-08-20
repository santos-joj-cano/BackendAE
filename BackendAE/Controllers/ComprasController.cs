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
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComprasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Compras
        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")]
        public async Task<ActionResult<IEnumerable<CompraDTO>>> GetCompras()
        {
            var compras = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.Usuario)
                .Include(c => c.DetalleCompras).ThenInclude(dc => dc.Producto)
                .ToListAsync();

            var comprasDTO = compras.Select(c => new CompraDTO
            {
                CompraId = c.CompraId,
                FechaCompra = c.FechaCompra,
                Total = c.Total,
                Proveedor = c.Proveedor,
                Usuario = new UsuarioSimpleDTO
                {
                    UsuarioId = c.Usuario.UsuarioId,
                    NombreCompleto = $"{c.Usuario.PrimerNombre} {c.Usuario.PrimerApellido}",
                    NombreUsuario = c.Usuario.NombreUsuario
                },
                DetallesCompra = c.DetalleCompras.Select(dc => new DetalleCompraDTO
                {
                    DetalleCompraId = dc.DetalleCompraId,
                    Cantidad = dc.Cantidad,
                    PrecioUnitario = dc.PrecioUnitario,
                    Subtotal = dc.Subtotal,
                    Producto = dc.Producto
                }).ToList()
            }).ToList();

            return comprasDTO;
        }

        // POST: api/Compras
        [HttpPost]
        [Authorize(Roles = "Admin, Empleado")]
        public async Task<ActionResult<CompraDTO>> PostCompra(CompraRequestDTO compraRequest)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal totalCompra = 0;
                var detallesCompra = new List<DetalleCompra>();

                foreach (var detalleRequest in compraRequest.DetallesCompra)
                {
                    var producto = await _context.Productos.FindAsync(detalleRequest.ProductoId);
                    if (producto == null)
                    {
                        await transaction.RollbackAsync();
                        return BadRequest($"Producto con Id {detalleRequest.ProductoId} no encontrado.");
                    }

                    producto.Stock += detalleRequest.Cantidad;

                    var detalleCompra = new DetalleCompra
                    {
                        ProductoId = detalleRequest.ProductoId,
                        Cantidad = detalleRequest.Cantidad,
                        PrecioUnitario = producto.PrecioAdquisicion,
                        Subtotal = detalleRequest.Cantidad * producto.PrecioAdquisicion,
                    };
                    detallesCompra.Add(detalleCompra);
                    totalCompra += detalleCompra.Subtotal;
                }

                var compra = new Compra
                {
                    ProveedorId = compraRequest.ProveedorId,
                    UsuarioId = compraRequest.UsuarioId,
                    FechaCompra = DateTime.Now,
                    Total = totalCompra,
                    DetalleCompras = detallesCompra
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                var compraGuardada = await _context.Compras
                    .Include(c => c.Usuario)
                    .Include(c => c.Proveedor)
                    .Include(c => c.DetalleCompras).ThenInclude(dc => dc.Producto)
                    .FirstOrDefaultAsync(c => c.CompraId == compra.CompraId);

                var compraDTO = new CompraDTO
                {
                    CompraId = compraGuardada.CompraId,
                    FechaCompra = compraGuardada.FechaCompra,
                    Total = compraGuardada.Total,
                    Proveedor = compraGuardada.Proveedor,
                    Usuario = new UsuarioSimpleDTO
                    {
                        UsuarioId = compraGuardada.Usuario.UsuarioId,
                        NombreCompleto = $"{compraGuardada.Usuario.PrimerNombre} {compraGuardada.Usuario.PrimerApellido}",
                        NombreUsuario = compraGuardada.Usuario.NombreUsuario
                    },
                    DetallesCompra = compraGuardada.DetalleCompras.Select(dc => new DetalleCompraDTO
                    {
                        DetalleCompraId = dc.DetalleCompraId,
                        Cantidad = dc.Cantidad,
                        PrecioUnitario = dc.PrecioUnitario,
                        Subtotal = dc.Subtotal,
                        Producto = dc.Producto
                    }).ToList()
                };

                return CreatedAtAction("GetCompra", new { id = compra.CompraId }, compraDTO);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // PUT: api/Compras/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutCompra(int id, CompraRequestDTO compraRequest)
        {
            if (!CompraExists(id))
            {
                return NotFound();
            }

            // Solo actualizar los campos que no son parte de la transacción de inventario
            var compraExistente = await _context.Compras.FindAsync(id);

            compraExistente.ProveedorId = compraRequest.ProveedorId;
            compraExistente.UsuarioId = compraRequest.UsuarioId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompraExists(id))
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

        // DELETE: api/Compras/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCompra(int id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }

            // En un sistema real, sería mejor un 'soft-delete'
            // compra.Estado = "Anulada";
            // _context.Entry(compra).State = EntityState.Modified;

            // Borrado físico de la compra y sus detalles
            var detallesCompra = await _context.DetalleCompras.Where(dc => dc.CompraId == id).ToListAsync();
            _context.DetalleCompras.RemoveRange(detallesCompra);

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Método de soporte para verificar si una compra existe
        private bool CompraExists(int id)
        {
            return _context.Compras.Any(e => e.CompraId == id);
        }
    }
}
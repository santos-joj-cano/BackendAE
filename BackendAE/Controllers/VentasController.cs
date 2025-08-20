using BackendAE.Data;
using BackendAE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendAE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VentasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<IEnumerable<Venta>>> GetVentas()
        {
            return await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.CajaSesion)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<ActionResult<Venta>> GetVenta(int id)
        {
            var venta = await _context.Ventas
                .Include(v => v.Usuario)
                .Include(v => v.CajaSesion)
                .FirstOrDefaultAsync(v => v.VentaId == id);
            if (venta == null)
            {
                return NotFound();
            }
            return venta;
        }

        // PUT: api/Ventas/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        public async Task<IActionResult> PutVenta(int id, Venta venta)
        {
            if (id != venta.VentaId)
            {
                return BadRequest();
            }
            _context.Entry(venta).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VentaExists(id))
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

        //// POST: api/Ventas
        //[HttpPost]
        //[Authorize(Roles = "Admin, Empleado")] // Access para Admin y Empleado
        //public async Task<ActionResult<Venta>> PostVenta(Venta venta)
        //{
        //    _context.Ventas.Add(venta);
        //    await _context.SaveChangesAsync();
        //    return CreatedAtAction("GetVenta", new { id = venta.VentaId }, venta);
        //}
        // POST: api/Ventas
        [HttpPost]
        [Authorize(Roles = "Admin, Empleado")]
        public async Task<ActionResult<Venta>> PostVenta(Venta venta)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Generar CódigoVenta único
                venta.CodigoVenta = "VTA-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

                // 2. Calcular el total de la venta
                decimal totalVenta = 0;
                foreach (var detalleVenta in venta.DetallesVentas)
                {
                    var producto = await _context.Productos.FindAsync(detalleVenta.ProductoId);
                    if (producto == null)
                    {
                        return BadRequest($"Producto con Id {detalleVenta.ProductoId} no encontrado.");
                    }

                    // Calcular el subtotal y sumarlo al total de la venta
                    detalleVenta.PrecioUnitario = producto.PrecioVenta;
                    detalleVenta.Subtotal = detalleVenta.Cantidad * detalleVenta.PrecioUnitario;
                    totalVenta += detalleVenta.Subtotal;

                    // 3. Actualizar el stock del producto
                    if (producto.Stock < detalleVenta.Cantidad)
                    {
                        return BadRequest($"Stock insuficiente para el producto {producto.Nombre}. Stock disponible: {producto.Stock}");
                    }
                    producto.Stock -= detalleVenta.Cantidad;
                }

                venta.Total = totalVenta;
                venta.Cambio = venta.EfectivoRecibido - venta.Total;
                venta.FechaVenta = DateTime.Now;

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction("GetVenta", new { id = venta.VentaId }, venta);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // DELETE: api/Ventas/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Solo acceso para Admin
        public async Task<IActionResult> DeleteVenta(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            _context.Ventas.Remove(venta);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.VentaId == id);
        }
    }
}
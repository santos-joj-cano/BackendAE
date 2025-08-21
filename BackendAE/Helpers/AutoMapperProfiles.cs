using AutoMapper;
using BackendAE.Models;
using BackendAE.DTOs;

namespace BackendAE.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Usuarios
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.NombreUsuario, opt =>
                    opt.MapFrom(src => $"{src.PrimerNombre} {src.PrimerApellido}"))
                .ForMember(dest => dest.RolNombre, opt =>
                    opt.MapFrom(src => src.Rol != null ? src.Rol.RolNombre : null));

            CreateMap<UsuarioCreacionDTO, Usuario>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            // Roles
            CreateMap<Rol, RolDTO>();
            CreateMap<RolDTO, Rol>();

            // Productos
            CreateMap<Producto, ProductoDTO>()
            .ForMember(dest => dest.NombreCategoria, opt =>
             opt.MapFrom(src => src.CategoriaProducto != null ? src.CategoriaProducto.Nombre : null));

            CreateMap<ProductoCreacionDTO, Producto>();

            // Categorías de Productos
            CreateMap<CategoriaProducto, CategoriaProductoDTO>();
            CreateMap<CategoriaProductoCreacionDTO, CategoriaProducto>();

            // Proveedores
            CreateMap<Proveedor, ProveedorDTO>()
            .ForMember(dest => dest.NombreCategoria, opt =>
            opt.MapFrom(src => src.CategoriaProveedor != null ? src.CategoriaProveedor.Nombre : null));

            CreateMap<ProveedorCreacionDTO, Proveedor>();

            // Categorías de Proveedores
            CreateMap<CategoriaProveedor, CategoriaProveedorDTO>();
            CreateMap<CategoriaProveedorCreacionDTO, CategoriaProveedor>();

            // Compras
            CreateMap<Compra, CompraDTO>()
    .ForMember(dest => dest.NombreProveedor, opt =>
        opt.MapFrom(src => src.Proveedor != null ? src.Proveedor.Empresa : null))
    .ForMember(dest => dest.DetalleCompras, opt =>
        opt.MapFrom(src => src.DetalleCompras));

            CreateMap<CompraCreacionDTO, Compra>();

            CreateMap<DetalleCompra, DetalleCompraDTO>()
                .ForMember(dest => dest.NombreProducto, opt =>
                    opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null));

            CreateMap<DetalleCompraCreacionDTO, DetalleCompra>();

            // Ventas
            CreateMap<Venta, VentaDTO>()
    .ForMember(dest => dest.NombreUsuario, opt =>
        opt.MapFrom(src => src.Usuario != null ? src.Usuario.NombreUsuario : null))
    .ForMember(dest => dest.CodigoVenta, opt =>
        opt.MapFrom(src => src.CajaSesion != null ? src.CajaSesion.Caja.Nombre : null))
    .ForMember(dest => dest.DetalleVentas, opt =>
        opt.MapFrom(src => src.DetalleVentas));

            CreateMap<VentaCreacionDTO, Venta>();

            CreateMap<DetalleVenta, DetalleVentaDTO>()
                .ForMember(dest => dest.NombreProducto, opt =>
                    opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null));

            CreateMap<DetalleVentaCreacionDTO, DetalleVenta>();

            // Categoria Producto
            CreateMap<CategoriaProducto, CategoriaProductoDTO>();
            CreateMap<CategoriaProductoCreacionDTO, CategoriaProducto>();
            // Caja
            CreateMap<Caja, CajaDTO>();
            CreateMap<CajaCreacionDTO, Caja>();
            // CajaSesion
            CreateMap<CajaSesion, CajaSesionDTO>()
                .ForMember(dest => dest.NombreCaja, opt =>
                    opt.MapFrom(src => src.Caja != null ? src.Caja.Nombre : null))
                .ForMember(dest => dest.NombreUsuarioApertura, opt =>
                    opt.MapFrom(src => src.UsuarioApertura != null ? src.UsuarioApertura.NombreUsuario : null))
                .ForMember(dest => dest.NombreUsuarioCierre, opt =>
                    opt.MapFrom(src => src.UsuarioCierre != null ? src.UsuarioCierre.NombreUsuario : null));
            // MovimientoCaja
            CreateMap<MovimientoCaja, MovimientoCajaDTO>()
                .ForMember(dest => dest.Tipo, opt =>
                    opt.MapFrom(src => src.CajaSesion != null && src.CajaSesion.Caja != null ? src.CajaSesion.Caja.Nombre : null))
                .ForMember(dest => dest.NombreUsuario, opt =>
                    opt.MapFrom(src => src.CajaSesion != null && src.CajaSesion.UsuarioApertura != null ? src.CajaSesion.UsuarioApertura.NombreUsuario : null));
            // DetalleCompra
            CreateMap<DetalleCompra, DetalleCompraDTO>()
                .ForMember(dest => dest.NombreProducto, opt =>
                    opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null));
            // DetalleVenta
            CreateMap<DetalleVenta, DetalleVentaDTO>()
                .ForMember(dest => dest.NombreProducto, opt =>
                    opt.MapFrom(src => src.Producto != null ? src.Producto.Nombre : null));

            // Aquí irían más mapeos (Producto, Categoria, Proveedor, etc.)
        }
    }
}

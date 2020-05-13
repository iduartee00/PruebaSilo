using Microsoft.EntityFrameworkCore;

namespace tienda_web.Models
{
    public class TiendaContext: DbContext
    {
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        public TiendaContext(DbContextOptions<TiendaContext> options) : base(options)
        {

        }
    }
}
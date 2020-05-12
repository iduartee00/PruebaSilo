using Microsoft.EntityFrameworkCore;

namespace informatica_web.Models
{
    public class InformaticaContext: DbContext
    {
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Brands> Brands { get; set; }
        public DbSet<Informatico> Informaticos { get; set; }
        
        //¿Debe agregarse la tabla InformTips?
        public DbSet<ReporteInv> ReporteInv { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<Lending> Lendings { get; set; }
        public DbSet<Empleados> Empleados { get; set; }

        public InformaticaContext(DbContextOptions<InformaticaContext> options) : base(options)
        {

        }
    }
}
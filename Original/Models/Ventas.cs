using System;

namespace tienda_web.Models
{
    public class Venta
    {
        public int VentaId { get; set; }
        public double FlAmout { get; set; }
        public int UsuarioId { get; set; }
        public DateTime DtVenta { get; set; }
    }
}
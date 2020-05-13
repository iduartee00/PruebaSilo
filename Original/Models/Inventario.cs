using System;

namespace tienda_web.Models
{
    public class Inventario
    {
        public int InventarioId{ get; set; }
        public int ProductoId{ get; set; }
        public DateTime DtIngreso{ get; set; }
        public DateTime DtCaducidad{ get; set; }
        public string VcVendido{ get; set; }
    }
}
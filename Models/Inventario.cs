using System;

namespace informatica_web.Models
{
    public class Inventario
    {
        public int InventarioId{ get; set; }
        public int InformId{ get; set; }
        public DateTime DtIngreso{ get; set; }
        public int InvQuant{ get; set; }
    }
}
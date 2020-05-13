using System;

namespace tienda_web.Models
{
    public class Factura
    {
        public int FacturaId{ get; set; }
        public int VentaId{ get; set; }
        public double FlAmout{ get; set; }
        public string VcUsrRfc{ get; set; }
        public string VcUsrNombre{ get; set; }
        public DateTime DtVenta{ get; set; }
        public string VcMarcaName{ get; set; }
        public string VcProvName{ get; set; }
        public string VcProdName{ get; set; }
        public double FlUnitAmount{ get; set; }
    }
}
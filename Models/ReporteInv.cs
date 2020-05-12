using System;

namespace informatica_web.Models
{
    public class ReporteInv
    {
        public int RepInventId{ get; set; }
        public int LendId{ get; set; }
        public string VcEmpsRfc{ get; set; }
        public string VcEmpsNombre{ get; set; }
        public DateTime DtLend{ get; set; }
        public string VcBrandName{ get; set; }
        public string VcProvName{ get; set; }
        public string InformDesc{ get; set; }
    }
}
namespace informatica_web.Models
{
    public class Informatico
    {
        public int InformId{ get; set; }
        public int InformTipId { get; set; }
        public string InformDesc{ get; set; }
        public int ProveedorId{ get; set; }
        public int BrandId{ get; set; }
        public string VcInforStatus{ get; set; }
    }
}
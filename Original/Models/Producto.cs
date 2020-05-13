namespace tienda_web.Models
{
    public class Producto
    {
        public int ProductoId{ get; set; }
        public string VcProdName{ get; set; }
        public int ProveedorId{ get; set; }
        public int MarcaId{ get; set; }
        public string VcProdStatus{ get; set; }
        public double FlUnitAmount{ get; set; }
        public double FlLoteAmount{ get; set; }
        public int IntCantLote{ get; set; }
    }
}
using System;

namespace informatica_web.Models
{
    public class Lending
    {
        public int LendId { get; set; }
        public int LendCant { get; set; }
        public int EmpleadoId { get; set; }
        public DateTime DtLend { get; set; }
    }
}
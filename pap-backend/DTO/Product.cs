using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pap_backend.DTO
{
    public class Product
    {
        public int idProd { get; set;}

        public string nameProd {get; set;}
        public decimal price { get; set; }
        public string resourceImg { get; set; }

        public List<decimal> availableSizes { get; set; } = new List<decimal>();
    }
}

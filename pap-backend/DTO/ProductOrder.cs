using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pap_backend.DTO
{
    public class ProductOrder
    {
        public string UserEmail { get; set; }
   
        public List<Product> Products { get; set; } = new List<Product>();
    }
}

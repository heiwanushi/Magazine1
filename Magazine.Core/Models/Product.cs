using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magazine.Core.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }  // Изменено на string
    }
}

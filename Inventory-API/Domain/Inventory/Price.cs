using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory
{
    public class Price
    {
        public string? PriceId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }
}

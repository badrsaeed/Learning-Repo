using StratgyPattern.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratgyPattern.Business.Strategies.SalesTax
{
    internal class UsaStateSalesTaxStrategy : ISalesTaxStrategy
    {
        public decimal GetTaxFor(Order order)
        {
            switch (order.ShippingDetails.DestinationCountry.ToLowerInvariant())
            {
                case "la": return order.TotalPrice * .095m;
                case "ny": return order.TotalPrice * .04m;
                case "nyc": return order.TotalPrice * .045m;

                default: return 0m;
            }
        }
    }
}

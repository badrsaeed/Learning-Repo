using StratgyPattern.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratgyPattern.Business.Strategies.SalesTax
{
    internal class SwedenSalesTaxStrategy : ISalesTaxStrategy
    {
        public decimal GetTaxFor(Order order)
        {
            var destination = order.ShippingDetails.DestinationCountry.ToLowerInvariant();
            decimal totalTax = 0m;
            var origin = order.ShippingDetails.OriginCountry.ToLowerInvariant();

            #region Tax Per Item
            foreach (var item in order.LineItems)
            {
                
                switch (item.Key.ItemType)
                {
                    case ItemType.Literature:
                        totalTax += item.Key.Price * .06m * item.Value;
                        break;
                    case ItemType.Service:
                        totalTax += item.Key.Price * .08m * item.Value;
                        break;
                    case ItemType.Food:
                        totalTax += item.Key.Price * .04m * item.Value;
                        break;
                    case ItemType.Hardware:
                        totalTax += item.Key.Price * .1m * item.Value;
                        break;
                    default:
                        break;
                }
            }
            
            #endregion
            return totalTax;
        }
    }
}

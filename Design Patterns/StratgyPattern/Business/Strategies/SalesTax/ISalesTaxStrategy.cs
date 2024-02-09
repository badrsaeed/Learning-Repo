using StratgyPattern.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratgyPattern.Business.Strategies.SalesTax
{
    public interface ISalesTaxStrategy
    {
        decimal GetTaxFor (Order order);
    }
}

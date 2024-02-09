using StratgyPattern.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratgyPattern.Business.Strategies.Invoice
{
    public interface IInvoiceStrategy
    {
        void Generate(Order order);
    }
}

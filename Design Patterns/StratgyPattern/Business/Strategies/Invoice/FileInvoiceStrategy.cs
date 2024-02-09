using StratgyPattern.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StratgyPattern.Business.Strategies.Invoice
{
    public class FileInvoiceStrategy : InvoiceStrategy
    {
        public override void Generate(Order order)
        {
            using(var stream = new StreamWriter($"invoice_{Guid.NewGuid()}.txt"))
            {
                stream.WriteLine(GenerateTextInvoice(order));
                stream.Flush();
            }
        }
    }
}

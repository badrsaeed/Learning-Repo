using StratgyPattern.Business.Models;
using StratgyPattern.Business.Strategies.Invoice;
using StratgyPattern.Business.Strategies.SalesTax;

namespace StratgyPattern
{
    public class Program
    {
        static void Main(string[] args)
        {
            var order = new Order
            {
                ShippingDetails = new ShippingDetails
                {
                    DestinationCountry = "Sweden",
                    OriginCountry = "Sweeden"
                }
            };

            order.LineItems?.Add(
                new Item(
                    "CSHARP", "C # Book", 100m, ItemType.Literature ) , 1
                );
            order.LineItems?.Add(
                new Item("Consulting", "Building a Website", 100m, ItemType.Service),2
                );

            Console.WriteLine(order.GetTax(new SwedenSalesTaxStrategy()));
            order.FinalizeOrder(new EmailInvoiceStrategy());
        }
      
    }
}
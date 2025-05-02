
namespace Solid.Models
{
    internal class OrderManagement
    {
        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"Processing Order {order.GetName()} now...");
        }
    }
}

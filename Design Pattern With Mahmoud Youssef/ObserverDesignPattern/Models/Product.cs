namespace ObserverDesignPattern.Models
{
    public class Product
    {
        public double Price { get; }
        public string Name { get; }

        public Product(double price, string name)
        {
            Price = price;
            Name = name;
        }
    }
}
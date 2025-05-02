namespace ObserverDesignPattern.Models
{
    public class User
    {
        public User(string name, bool isSubscribedToProducts, bool isSubscribedToOffers)
        {
            Name = name;
            IsSubscribedToProducts = isSubscribedToProducts;
            IsSubscribedToOffers = isSubscribedToOffers;
        }

        public string Name { get; }
        public bool IsSubscribedToProducts { get; }
        public bool IsSubscribedToOffers { get; }

        public void Notify(Product product)
        {
            Console.WriteLine($"Notifying user: {Name} by product : {product.Name}");
        }
        public void Notify(Offer offer)
        {
            Console.WriteLine($"Notifying user: {Name} by product : {offer.Message}");
        }
    }
}
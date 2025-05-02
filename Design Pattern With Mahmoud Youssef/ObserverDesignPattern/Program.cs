using ObserverDesignPattern.Models;

namespace ObserverDesignPattern
{
    internal class Program
    {
        static void Main(string[] args)
        {
            OnlineMarketPlace onlineMarketPlace = new OnlineMarketPlace();

            onlineMarketPlace.Users.Add(
                new User("Badr", true, true));
            onlineMarketPlace.Users.Add(
                new User("Ahmed", false, true));
            onlineMarketPlace.Users.Add(
                new User("Ali", true, false));
            onlineMarketPlace.Users.Add(
                new User("Amr", false, false)
                );


            onlineMarketPlace.AddNewProduct(new Product(50, "Mobile"));
            onlineMarketPlace.AddNewOffer(new Offer("Offer with 50% discount!!!!"));
        }
    }
}

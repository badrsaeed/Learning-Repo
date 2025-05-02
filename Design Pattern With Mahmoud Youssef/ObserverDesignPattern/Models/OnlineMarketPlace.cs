using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObserverDesignPattern.Models
{
    public class OnlineMarketPlace 
    {
        public List<User> Users { get; set; }
        public List<Product> Products { get; set; }
        public List<Offer> Offers { get; set; }

        public OnlineMarketPlace()
        {
            Users = new List<User>();
            Products = new List<Product>();
            Offers = new List<Offer>();
        }

        public void AddNewProduct(Product product)
        {
            Products.Add(product);

            NotifyUser(product);    
        }

        private void NotifyUser(Product product)
        {
            foreach (var user in Users)
            {
                if (user.IsSubscribedToProducts)
                    user.Notify(product);
            }
        }

        public void AddNewOffer(Offer offer)
        {
            Offers.Add(offer);
            NotifyUser(offer);
        }

        private void NotifyUser(Offer offer)
        {
            foreach (var user in Users)
            {
                if(user.IsSubscribedToOffers)
                    user.Notify(offer);
            }
        }
    }
}

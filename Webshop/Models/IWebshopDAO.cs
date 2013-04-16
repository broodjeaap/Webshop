using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public interface IWebshopDAO
    {
        List<Product> getProducts(string category = "", string subcat1 = "", string subcat2 = "", int page = 1, int perPage = 10);
        List<Product> getSearchResults(string query);
        Product getProduct(int id);

        int getCurrentUserId();
        User getCurrentUser();
        bool getCurrentUserIsAdmin();
        User getUser(int id);        
        Order getOrder(int id);
        List<ShoppingCartItem> getCurrentUsersShoppingCartItems();

        List<Ticket> getAllTicketsOrderedByDate();
        List<UserTicketLink> getUserTicketsOrderedByDate();
        List<Ticket> getNewTicketsOrderedByDate();
        Ticket getTicket(int id);
        List<User> getHelpUsers();
    }
    /*
     * + * queries verhuizen naar DAO layer
+ * inheretence
+ * inserts verhuizen naar DSO, service layer

*/
}
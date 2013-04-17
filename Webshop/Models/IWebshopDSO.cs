using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webshop.Models
{
    public interface IWebshopDSO
    {
        bool createUser(string email, string password);
        bool createUser(string email, string password, string type);
        bool createUser(string email, string password, UserType type);
        bool adminCreateUser(string email, string password, string type);
        bool adminCreateUser(string email, string password, UserType type);
        bool changeUserType(int user, string type);
        bool changeUserType(User user, string type);
        bool changeUserType(User user, UserType type);
        bool addAddressToCurrentUser(Address address);
        bool changeAddress(Address address);
        bool editProduct(Product product);
        bool createTicket(Ticket ticket);
        List<string> getDistinctCategories();
        List<string> getDistinctSubCat1(string category);
        List<string> getDistinctSubCat2(string category, string subcat1);
        bool addShoppingCartItem(int id, int quantity = 1);
        bool updateShoppingCartItem(int id, int quantity);
        bool deleteShoppingCartItem(int id, int quantity);
        bool placeOrderToAddress(int id);
        bool setTicketLastViewedByCurrentUserToNow(int ticketID);
        bool currentUserCanSeeTicket(int id);
        bool currentUserCanSeeTicket(Ticket ticket);
        bool postCommentOnTicket(TicketComment comment);
        bool changeTicketState(int id, string newState);
        bool assignUserToTicket(int userID, int ticketID);
    }
}
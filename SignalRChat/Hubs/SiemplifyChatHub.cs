using System;
using System.Web;
using Microsoft.AspNet.SignalR;


namespace SignalRChat
{
    // Core class that intercepts all messages from all clients
    public class SiemplifyChatHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

        public void LoadHistory(string userName)
        {
            //Clients.Others.popup(msg);
        }

        public void AddUser(string name)
        {

        }
    }
}
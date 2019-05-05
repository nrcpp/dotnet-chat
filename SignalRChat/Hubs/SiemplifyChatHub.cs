using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using SignalRChat.DAL;

namespace SignalRChat
{
    // Core class that intercepts all messages from all clients
    public class SiemplifyChatHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);

            // save to history

        }

        public override Task OnConnected()
        {            
            // load all contacts            
            foreach (var c in ChatData.Instance.Users)
                AddContact(c.Name, "[DEP.]");

            return base.OnConnected();
        }


        public void AddContact(string name, string department)
        {
            Clients.All.addContact(name, department);
        }

        public void LoadHistory(string userName)
        {
            // load history of personal chat
            var user = ChatData.Instance.Users.Where(u => u.Name == userName).FirstOrDefault();
            if (user == null) return;


        }
    }
}
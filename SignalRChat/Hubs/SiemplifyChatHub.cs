using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        static ConcurrentDictionary<string, ChatData.User> _userConnections = new ConcurrentDictionary<string, ChatData.User>();

        private string GetClientIdByName(string name)
        {
            return _userConnections.Where(u => u.Value.Name == name).FirstOrDefault().Value?.ConnectionId;            
        }

        public void Send(string name, string contact, string message)
        {

            // Call the addNewMessageToPage method to update clients.
            if (contact == "All")
                Clients.All.addNewMessageToPage(name, message);
            else
            {
                var connectionId = GetClientIdByName(contact);
                if (connectionId == null)
                    return;

                Clients.Client(connectionId).addNewMessageToPage(name, message);
                Clients.Caller.addNewMessageToPage(name, message);
            }

            // save to history            
            ChatData.AddMessageToHistory(name, contact, message);            
        }

        public override Task OnConnected()
        {           
            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {            
            return base.OnDisconnected();
        }

        public void Connect(string name)
        {
            var pair = _userConnections.FirstOrDefault(u => u.Value.Name == name);
            if (pair.Value != null)
            {
                if (!_userConnections.TryRemove(pair.Key, out ChatData.User value))
                    return;
            }

            var newUser = new ChatData.User()
            {
                Name = name,
                ConnectionId = Context.ConnectionId,
            };

            if (!_userConnections.TryAdd(name, newUser))
                return;

            // Update contacts list of other clients
            Clients.Others.clearContacts();

            // add All contact, which will broadcast all messsages
            Clients.All.addContact("All", "DEP");
            Clients.Others.addContact(name, "DEP");        // front-end won't add if it is already in list            

            // and update Caller contact list
            foreach (var u in _userConnections.Values)
            {
                if (u.Name != name)
                    Clients.Caller.addContact(u.Name, "DEP");
            }

        }
    }
}
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

        public static void ActivateChat(string userName, string contactName)
        {
            var user = GetContactByName(userName);
            var contact = GetContactByName(contactName);

            if (user != null)
                user.ActiveContact = contact;

            if (contact != null)
                contact.ActiveContact = user;
        }

        private static ChatData.User GetContactByName(string name)
        {
            name = name.ToLower().Trim();
            return _userConnections.Where(u => u.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;            
        }

        public void Send(string name, string contact, string message)
        {
            string time = DateTime.Now.ToString();

            // Call the addNewMessageToPage method to update clients.
            if (contact == "All")
            {
                // we filter messages with active contacts, so it won't appear on personal chats
                Clients.All.addNewMessageToPage(name, contact, message, time);
            }

            else
            {
                var contactUser = GetContactByName(contact);
                if (contactUser?.ConnectionId != null)
                {
                    // here we have to check, if receiever is active
                    if (contactUser.ActiveContact?.Name == name)
                        Clients.Client(contactUser.ConnectionId).addNewMessageToPage(name, contact, message, time);
                }                

                Clients.Caller.addNewMessageToPage(name, contact, message, time);
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

        public void Connect(string name, string contact)
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

            ActivateChat(name, contact);

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
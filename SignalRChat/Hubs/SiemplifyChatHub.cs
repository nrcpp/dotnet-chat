using System;
using System.Collections.Concurrent;
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
            return _userConnections.FirstOrDefault(u => u.Key == name).Value?.ConnectionId;
        }

        public void Send(string myName, string message)
        {
            string contactName = "All";

            // Call the addNewMessageToPage method to update clients.
            if (contactName == "All")
                Clients.All.addNewMessageToPage(myName, message);

            else
                Clients.Client(GetClientIdByName(contactName)).addNewMessageToPage(myName, message);

            // save to history
            if (_userConnections.TryGetValue(Context.ConnectionId, out ChatData.User user))
            {
                user.History.Messages.Add(new ChatData.Message()
                {
                    FromUser = contactName,
                    Content = message,
                    Time = DateTime.Now.ToString(),
                });
            }
        }

        public override Task OnConnected()
        {
            // load all contacts            
            //foreach (var contact in ChatData.Instance.Users)
            //{
            //    var offlineUser = _userConnections.SingleOrDefault(u => u.Key == Context.ConnectionId).Value;
            //    if (offlineUser == null)
            //    {
            //        contact.ConnectionId = Context.ConnectionId;
            //        AddContact(contact.Name, "[DEP.]");

            //        if (_userConnections.ContainsKey(contact.Name))
            //            _userConnections[contact.Name] = contact;
            //        else
            //        {
            //            bool ok = _userConnections.TryAdd(contact.Name, contact);
            //        }
            //    }
            //}

            // load history


            return base.OnConnected();
        }

        public override Task OnDisconnected()
        {
            //_userConnections.TryRemove(Context.ConnectionId, out ChatData.User user);

            return base.OnDisconnected();
        }

        public void Connect(string name)
        {
            var user = _userConnections.SingleOrDefault(u => u.Key == name).Value;
            var newUser = new ChatData.User()
            {
                Name = name,
                ConnectionId = Context.ConnectionId,
            };

            if (!_userConnections.TryAdd(Context.ConnectionId, newUser))
                return;

            // Update contacts list of other clients
            Clients.Others.clearContacts();
            Clients.Others.addContact(name, "DEP.");        // front-end won't add if it is already in list

            // add All contact, which will broadcast all messsages
            Clients.Caller.addContact("All", "");

            // and update Caller contact list
            foreach (var u in _userConnections.Values)
            {
                if (u.Name != name)
                    Clients.Caller.addContact(u.Name, "DEP.");
            }

        }
    }
}
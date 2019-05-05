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
        
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);

            // save to history
            if (_userConnections.TryGetValue(Context.ConnectionId, out ChatData.User user))
            {
                user.History.Messages.Add(new ChatData.Message()
                {
                    FromUser = name,
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

            if (_userConnections.TryAdd(Context.ConnectionId, newUser))
            {
                Clients.Others.clearContacts();
                Clients.Others.addContact(name, "DEP.");        // front-end won't add if it is already in list

                // and update Caller contact list
                foreach (var u in _userConnections.Values)
                {                    
                    if (u.Name != name)
                        Clients.Caller.addContact(u.Name, "DEP.");                    
                }
            }
        }

        public void LoadHistory(string userName)
        {
            // load history of personal chat
            var user = ChatData.Instance.Users.Where(u => u.Name == userName).FirstOrDefault();
            if (user == null) return;


        }
    }
}
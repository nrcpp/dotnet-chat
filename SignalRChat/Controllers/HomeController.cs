
using SignalRChat.DAL;
using SignalRChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SignalRChat.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        private ChatData.User GetOrRegisterUser(string name)
        {
            var user = ChatData.Instance.Users.FirstOrDefault(u => u.Name == name);
            if (user != null)
                return user;

            // register
            else if (!string.IsNullOrWhiteSpace(name))
            {
                user = new ChatData.User()
                {
                    Name = name,
                };

                ChatData.Instance.Users.Add(user);
                ChatData.Instance.Save();

                return user;
            }

            else
                return null;
        }

        private string GenerateHistory(ChatData.User from, ChatData.User to)
        {
            var list = from.History.Messages.ConvertAll<string>(m => m.Content);
            string result = "";

            foreach (var s in list)            
                result += $"<li><strong>{s}</li></strong>";            

            return result;
        }

        public ActionResult Chat(string name, string contact = "All")
        {
            var user = GetOrRegisterUser(name);
            if (user == null)
                return View("Index");

            var toUser = GetOrRegisterUser(contact);
            if (toUser == null)
                return View("Index");

            var model = new ChatModel
            {
                User = user,
                Contact = toUser,
                History = GenerateHistory(user, toUser),
            };

            return View(model);
        }
    }
}
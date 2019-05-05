
using SignalRChat.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult Chat(string name)
        {
            var user = GetOrRegisterUser(name);
            if (user == null)
                return View("Index");

            return View(user );
        }


        public ActionResult PersonalChat(string name, string contact, string department)
        {
            var user = ChatData.Instance.Users.FirstOrDefault(u => u.Name == contact);  // TODO: == name
            return View("Chat", user);          // TODO: create view and show personal chat
        }
    }
}
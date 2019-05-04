
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

        public ActionResult Chat(string name)
        {
            var user = ChatData.Instance.Users.FirstOrDefault(u => u.Name == name);
            if (user != null)
            {
                // test update history
                // TODO: remove user
                user.History.Messages.Add(new ChatData.Message()
                {
                    Content = "random content " + Guid.NewGuid().ToString().Substring(4),
                    Time = DateTime.Now.ToString(),
                    FromUser = "random user <b>" + Guid.NewGuid().ToString().Substring(4)
                });

                ChatData.Instance.Save();
            }

            else if (!string.IsNullOrWhiteSpace(name))
            {
                user = new ChatData.User()
                {
                    Name = name,
                };
                ChatData.Instance.Users.Add(user);

                ChatData.Instance.Save();
            }

            return View(user );
        }
    }
}
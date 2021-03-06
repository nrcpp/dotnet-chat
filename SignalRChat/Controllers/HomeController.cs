﻿
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


        private bool IsMessageInHistory(ChatData.Message historyMsg, string name, string contact)
        {
            if (historyMsg.FromUser == name && historyMsg.ToUser == contact) return true;

            if (historyMsg.ToUser == name && historyMsg.FromUser == contact) return true;

            if (contact == "All" && historyMsg.ToUser == "All") return true;
            
            return false;
        }


        public ActionResult Chat(string name, string contact = "All")
        {
            var user = GetOrRegisterUser(name);
            if (user == null)
                return View("Index");

            var contactUser = GetOrRegisterUser(contact);
            if (contactUser == null)
                return View("Index");

            var model = new Models.ChatModel()
            {
                User = user,
                Contact = contactUser,                
                History = ChatData.Instance.HistoryMessages.Where(m => IsMessageInHistory(m, name, contact)).ToList(),        // history for this user with the name 'name'
            };

            return View(model);
        }
    }
}
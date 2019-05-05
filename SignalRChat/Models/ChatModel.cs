using SignalRChat.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat.Models
{
    public class ChatModel
    {
        public ChatData.User User { get; set; } 
        public ChatData.User Contact { get; set; }
        public string History { get; set; } = "";
    }
}
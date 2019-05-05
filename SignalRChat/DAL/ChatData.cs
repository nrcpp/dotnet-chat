using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace SignalRChat.DAL
{
    [XmlRoot("ChatData")]
    public class ChatData
    {
        readonly static ChatData _instance = Load();
        public static ChatData Instance => _instance;
        
        public class Message
        {
            [XmlElement("Content")]
            public string Content { get; set; } = "";

            [XmlElement("Time")]
            public string Time { get; set; } = "";

            [XmlElement("ContactUser")]
            public string ContactUser { get; set; } = "";
        }


        // 
        public class User
        {
            [XmlElement("Name")]
            public string Name { get; set; } = "";

            [XmlElement("Status")]
            public string Status { get; set; } = OfflineStatus;

            [XmlArray("HistoryMessages")]            
            public List<Message> HistoryMessages { get; set; } = new List<Message>();

            [XmlArray("Contacts")]            
            public List<string> Contacts { get; set; } = new List<string>();

            [XmlIgnore]
            public string ConnectionId { get; set; } = "";
        }


        [XmlElement("Users")]
        public List<User> Users { get; set; } = new List<User>();

        public const string OnlineStatus = "online";
        public const string OfflineStatus = "offline";

        private static string DataFile => System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/chat-data.xml");

        public static void AddUserIfNotExists(User user)
        {
            var existing = Instance.Users.FirstOrDefault(u => user.Name == u.Name);
            if (existing != null)
            {
                return;
            }

            Instance.Users.Add(user);
            Instance.Save();        
        }

        public static void AddMessageToHistory(string userName, string contatName, string message)
        {
            var user = Instance.Users.FirstOrDefault(u => u.Name == userName);
            var from = Instance.Users.FirstOrDefault(u => u.Name == contatName);

            if (user != null)
            {
                user.HistoryMessages.Add(new Message()
                {
                    Content = message,
                    ContactUser = contatName,                    
                    Time = DateTime.Now.ToString(),
                });
            }

            if (from != null)
            {
                from.HistoryMessages.Add(new Message()
                {
                    Content = message,
                    ContactUser = userName,                    
                    Time = DateTime.Now.ToString(),
                });
            }

            Instance.Save();
        }

        #region Save/Load and serialization


        private string Serialize()
        {
            var value = this;
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(ChatData));
                var stringWriter = new StringWriter();
                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                settings.Indent = true;
                settings.NewLineOnAttributes = true;

                using (var writer = XmlWriter.Create(stringWriter, settings))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An xml serialization error occurred", ex);
            }
        }


        private static ChatData Deserialize(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ChatData));
            using (TextReader reader = new StringReader(xml))
            {
                var result = (ChatData)serializer.Deserialize(reader);
                return result;
            }
        }


        public void Save()
        {
            string xml = Serialize();
            File.WriteAllText(DataFile, xml);
        }

        public static ChatData Load()
        {
            try
            {

                string file = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/chat-data.xml");
                if (!File.Exists(file)) return new ChatData();

                string xml = System.IO.File.ReadAllText(DataFile);
                var result = Deserialize(xml);

                if (result?.Users?.Any(u => u.Name == "All") == false)
                {
                    result.Users.Add(new User()
                    {
                        Name = "All"
                    });                    
                }

                return result;
            }
            catch (Exception)
            {
                return new ChatData();
            }
        }

        #endregion
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using xChatLib.Server;

namespace xChat_Server
{
    class Program
    {
        static string filePath = string.Empty;
        static void Main(string[] args)
        {
            if (!System.IO.File.Exists(AppDomain.CurrentDomain.BaseDirectory + "config.json"))
            {
                filePath = AppDomain.CurrentDomain.BaseDirectory + "config-default.json";
            }
            Configuration config = new Configuration();
            xChatServer s = new xChatLib.Server.xChatServer(config.readConfig(filePath));
           
            Console.Read();
        }
    }
}

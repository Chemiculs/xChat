using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using xChatLib.Networking;
namespace xChatLib.Server
{
    public class xChatServer
    {
        #region Objects
        private AsyncTcpServer _Server = null;
        private Config _config = null;
        #endregion
        #region Accessors
        public AsyncTcpServer Server {
            get { return _Server; }
            set { _Server = value; }
        }
        public Config Configuration
        {
            get { return _config; }
            set { _config = value; }
        }
        #endregion
        #region Constructors
        public xChatServer()
        {
            
        }
        public xChatServer(Config configuration)
        {
            try
            {
                Configuration = configuration;
                setupConsole();
                Console.WriteLine("Creating xChat Server Instance via Configuration given.");
                Console.WriteLine("Name:" + Configuration.name);
                Console.WriteLine("Port:" + Configuration.port);
                run();
            }
            catch (Exception ex)
            {
                ConsoleColor c = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Error: " + ex.Message);
                Console.ForegroundColor = c;

            }
        }
        #endregion
        #region Methods
        void setupConsole()
        {
            Console.Title = "xChat Server";
        }
        void run()
        {
            Server = new AsyncTcpServer(Configuration.port, Configuration.key);
        }
        #endregion
    }
}

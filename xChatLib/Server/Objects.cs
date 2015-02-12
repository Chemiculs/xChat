using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using xChatLib.Networking;
using xChatLib.Core.Events.Server;
using Newtonsoft.Json;
namespace xChatLib.Server
{
    public class xUser
    {
        #region Objects
		private AsyncClient _Client {get;set;} // Tcp Server object to accept connections from clients
		private string _Name {get;set;} // User name to display to other clients
        private string _Biography = string.Empty; // Biography for the user
        private Image _Image = null; // Profile image
        #endregion
        #region Accessors
		public string Name {
			get {
				return _Name;
			}
			set {
                _Name = value;
			}
		}
        public Image Image
        {
            get {
                return _Image;
            }
            set {
                _Image = value;
            }
        }
        #endregion
        #region Events / Delegates
        /// <summary>
        /// Fired when the user connects to a server.
        /// Uses the xChatLib.Core.Events.Server.UserConnected class.
        /// </summary>
        public event EventHandler<UserConnected> onConnect;
        /// <summary>
        /// Fired when the user disconnects from a server.
        /// </summary>
        public event EventHandler onDisconnect;
        /// <summary>
        /// Fired when a Heartbeat packet is received.
        /// </summary>
        public event EventHandler onHeartbeat;
        #endregion
        #region Constructors
        /// <summary>
        /// Default constructor
        /// </summary>
        public xUser() {
            Image = null;
            Name = null;

        }
        #endregion 
        #region Methods
        #endregion
    }
    public class Config
    {
        [JsonProperty("port")]
        public ushort port { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("welcomeMessage")]
        public string welcomeMessage { get; set; }
        [JsonProperty("whiteList")]
        public string whiteList { get; set; }
        [JsonProperty("blackList")]
        public string blackList {get; set;}
        [JsonProperty("key")]
        public string key { get; set; }
        [JsonProperty("admins")]
        public UInt32[] AdminIds { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
namespace xChatLib
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

			}
		}
        #endregion
        #region Events / Delegates
        #endregion
        #region Constructors
        #endregion 
        #region Methods
        #endregion
    }
}
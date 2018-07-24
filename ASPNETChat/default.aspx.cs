using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace ASPNETChat
{
	/// <summary>
	/// Summary description for WebForm1.
	/// </summary>
	public class WebForm1 : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox txtUserName;
		protected System.Web.UI.WebControls.Button btnLogin;
		protected System.Web.UI.WebControls.Panel pnlLogin;
		protected System.Web.UI.WebControls.TextBox txtOtherUser;
		protected System.Web.UI.WebControls.Button btnChat;
		protected System.Web.UI.WebControls.RequiredFieldValidator req1;
		protected System.Web.UI.WebControls.RequiredFieldValidator Requiredfieldvalidator1;
		protected System.Web.UI.WebControls.Panel pnlChat;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (Session["UserName"]!=null)
			{
				pnlLogin.Visible=false;
				pnlChat.Visible=true;
			}
			else
			{
				pnlLogin.Visible=true;
				pnlChat.Visible=false;
			}

		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			this.btnChat.Click += new System.EventHandler(this.btnChat_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void btnLogin_Click(object sender, System.EventArgs e)
		{
			Session["UserName"]=txtUserName.Text;
			pnlLogin.Visible=false;
			pnlChat.Visible=true;
		}

		private void btnChat_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("Chat.aspx?userid="+txtOtherUser.Text);
		}
	}
}

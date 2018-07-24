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
	public class Chat : System.Web.UI.Page
	{
		protected System.Web.UI.HtmlControls.HtmlInputHidden otherUser;
		protected System.Web.UI.WebControls.TextBox txtMsg;
		protected System.Web.UI.WebControls.Button btnExit;
		protected System.Web.UI.WebControls.Panel pnlChat;
		protected System.Web.UI.HtmlControls.HtmlTextArea txt;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (AMS.Web.RemoteScripting.InvokeMethod(this))//if this is a callback function return
				return;

			txtMsg.Attributes.Add("onkeypress", "return clickButton(event,'btn')");
			
			if (!IsPostBack)
			{
				if (Request.QueryString["userid"]!=null && Request.QueryString["userid"]!="")
				{
					otherUser.Value=Request.QueryString["userid"];
					ChatRoom room=ChatEngine.GetRoom(Session["UserName"].ToString() ,Request.QueryString["userid"]);
					string s=room.JoinRoom(Session["UserName"].ToString(),Session["UserName"].ToString() );
					txt.InnerText=s;
				//	string strScript="<script>startTimer();</script>";
				//	this.RegisterClientScriptBlock("timerScript",strScript);
				}
				else
				{
					Response.Write("User id Missing");
					pnlChat.Visible=false;
				}
			}
		}
	
		#region Script Callback functions

		/// <summary>
		/// This function is called from the client script 
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="toUserID"></param>
		/// <returns></returns>
		public string SendMessage(string msg,string toUserID)
		{
			try
			{
				ChatRoom room=ChatEngine.GetRoom(Session["UserName"].ToString() ,toUserID);
				string res="";
				if (room!=null)
				{
					res=room.SendMessage(msg,Session["UserName"].ToString() ,toUserID);
				}
				return res;
			}
			catch(Exception ex)
			{
				
			}
			return "";
		}


		/// <summary>
		/// This function is called peridically called from the user to update the messages
		/// </summary>
		/// <param name="otherUserID"></param>
		/// <returns></returns>
		public string UpdateUser(string otherUserID)
		{
			try
			{
				ChatRoom room=ChatEngine.GetRoom(Session["UserName"].ToString() ,otherUserID);
				if (room!=null)
				{
					string res="";
					if (room!=null)
					{
						res=room.UpdateUser(Session["UserName"].ToString());
					}
					return res;
				}
			}
			catch(Exception ex)
			{
				
			}
			return "";
		}


		/// <summary>
		/// This function is called from the client when the user is about to leave the room
		/// </summary>
		/// <param name="otherUser"></param>
		/// <returns></returns>
		public string LeaveRoom(string otherUser)
		{
			try
			{
				ChatRoom room=ChatEngine.GetRoom(Session["UserName"].ToString() ,otherUser);
				if (room!=null)
                    room.LeaveRoom(Session["UserName"].ToString() );
			}
			catch(Exception ex)
			{
				
			}
			return "";
		}
		#endregion

		#region Event Handlers
		private void btnExit_Click(object sender, System.EventArgs e)
		{
			try
			{
				ChatRoom room=ChatEngine.GetRoom(Session["UserName"].ToString() ,Request.QueryString["userid"]);
				room.LeaveRoom(Session["UserName"].ToString() );
			//	string  strScript = "<script language=javascript>self.close();</script>";
			//	RegisterClientScriptBlock("anything", strScript);
				Response.Redirect("default.aspx");
			}
			catch(Exception ex)
			{
				
			}
			
		}
		#endregion

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
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion
		
	}
}

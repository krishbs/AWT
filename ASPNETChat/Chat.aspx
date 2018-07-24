<%@ Page language="c#" Codebehind="Chat.aspx.cs" AutoEventWireup="false" Inherits="ASPNETChat.Chat" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Chat</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body leftMargin="0" topMargin="0" marginwidth="0" marginheight="0" onunload="Leave()">
		<form id="Form1" method="post" runat="server">
			<input id="otherUser" type="hidden" name="otherUser" runat="server">
			<script language="JavaScript" src="scripts/rs.js"></script>
			<script language="JavaScript">
			function button_clicked()
			{
			 	RS.Execute("Chat.aspx", "SendMessage",document.Form1.txtMsg.value,document.Form1.otherUser.value, callback, errorCallback);
				document.Form1.txtMsg.value="";
				document.Form1.txt.scrollIntoView("true");
			}
			function Leave()
			{
			RS.Execute("Chat.aspx", "LeaveRoom",document.Form1.otherUser.value);
			}
			function callback(result)
			{
				document.Form1.txt.value=document.Form1.txt.value+result;
				document.Form1.txt.doScroll();
			}
	  
			function errorCallback(result)
			{
				alert("An error occurred while invoking the remote method: " 
				+ result);
			}
			function clickButton(e, buttonid)
			{ 
				var bt = document.getElementById(buttonid); 
				if (typeof bt == 'object')
					{ 
						if(navigator.appName.indexOf("Netscape")>(-1)){ 
						if (e.keyCode == 13)
						{ 
							bt.click(); 
							return false; 
						} 
					} 
				if (navigator.appName.indexOf("Microsoft Internet Explorer")>(-1))
					{ 
						if (event.keyCode == 13)
						{ 
						    bt.click(); 
							return false; 
						} 
					} 
				}	 
			} 
			var interval = "";
			var i = 5;

			function startTimer()
			{
				interval = window.setInterval("tTimer()",1000);
			} 
			function stopTimer()
			{
				window.clearInterval (interval);
				interval="";
			}
			function tTimer()
			{
				RS.Execute("Chat.aspx", "UpdateUser",document.Form1.otherUser.value, callback, errorCallback);	
			}

			</script>
			<table cellSpacing="0" cellPadding="0" width="100%" border="0">
				<tr>
					<td vAlign="top" align="center"><br>
						<table width="100%" border="0">
							<tr>
								<td>
									<table cellSpacing="0" cellPadding="0" width="98%" align="center" border="0">
										<tr>
											<td align="center">
												<table id="Table4" cellSpacing="3" cellPadding="3" width="100%" bgColor="#f2f2f2" border="0">
													<tr align="center" bgColor="#f2f2f2">
														<td bgcolor="#f2f2f2">
															<table id="Table1" cellSpacing="3" cellPadding="0" width="98%" align="left" border="0">
																<asp:Panel Runat=server ID="pnlChat">
																<script>startTimer();</script>
																<TR>
																	<TD colSpan="6"><TEXTAREA id="txt" style="WIDTH: 690px; HEIGHT: 260px" name="txt" rows="16" cols="79" runat="server"></TEXTAREA>
																	</TD>
																</TR>
																<TR>
																	<TD colSpan="6">
																		<asp:TextBox id="txtMsg" Width="400" Runat="server"></asp:TextBox>&nbsp;&nbsp; <INPUT id="btn" onclick="button_clicked()" type="button" value="SEND">
																	</TD>
																</TR>
																<TR>
																	<TD>
																		<asp:Button id="btnExit" Runat="server" Text="EXIT"></asp:Button></TD>
																</TR>
																</asp:Panel>
															</table>
															<br>
														</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>

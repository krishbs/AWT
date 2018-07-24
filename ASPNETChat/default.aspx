<%@ Page language="c#" Codebehind="default.aspx.cs" AutoEventWireup="false" Inherits="ASPNETChat.WebForm1" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>WebForm1</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table>
				<asp:Panel Runat="server" ID="pnlLogin">
					<TBODY>
						<TR>
							<TD>User Name :
								<asp:TextBox id="txtUserName" Runat="server"></asp:TextBox>
								<asp:RequiredFieldValidator id="req1" Runat="server" ControlToValidate="txtUserName" ErrorMessage="8" Display="Dynamic"></asp:RequiredFieldValidator></TD>
						</TR>
						<TR>
							<TD>
								<asp:Button id="btnLogin" Runat="server" Text="LOGIN"></asp:Button></TD>
						</TR>
				</asp:Panel>
				<asp:Panel Runat="server" ID="pnlChat">
					<TR>
						<TD>Chat With:
							<asp:TextBox id="txtOtherUser" Runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator id="Requiredfieldvalidator1" Runat="server" ControlToValidate="txtOtherUser" ErrorMessage="8"
								Display="Dynamic"></asp:RequiredFieldValidator></TD>
					</TR>
					<TR>
						<TD>
							<asp:Button id="btnChat" Runat="server" Text="Chat"></asp:Button></TD>
					</TR>
				</asp:Panel></TBODY>
			</table>
		</form>
	</body>
</HTML>

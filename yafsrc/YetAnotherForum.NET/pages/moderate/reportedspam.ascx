<%@ Control Language="C#" AutoEventWireup="true" CodeFile="reportedspam.ascx.cs"
	Inherits="YAF.Pages.moderate.reportedspam" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="True" />
<YAF:PageLinks runat="server" ID="PageLinks" />
<asp:Repeater ID="List" runat="server">
	<HeaderTemplate>
		<table class="content" cellspacing="1" cellpadding="0" width="100%">
			<tr>
				<td colspan="2" class="header1" align="left">
					<%# PageContext.PageForumName %>
					-
					<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="REPORTED" />
				</td>
			</tr>
	</HeaderTemplate>
	<FooterTemplate>
		<tr>
			<td class="postfooter" colspan="2">
				&nbsp;</td>
		</tr>
		</table>
	</FooterTemplate>
	<ItemTemplate>
		<tr class="header2">
			<td colspan="2">
				<%# Eval("Topic") %>
			</td>
		</tr>
		<tr class="postheader">
			<td>
				<YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Convert.ToInt32(Eval("UserID")) %>'
					UserName='<%# Convert.ToString(Eval("UserName")) %>' />
			</td>
			<td>
				<b>
					<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="POSTED" />
				</b>
				<%# YafDateTime.FormatDateTime((System.DateTime) DataBinder.Eval(Container.DataItem, "[\"Posted\"]")) %>
				<b>
					<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NUMBERREPORTED" />
				</b>
				<%# DataBinder.Eval(Container.DataItem, "[\"NumberOfReports\"]") %>
				<label id="Label1" runat="server" visible='<%# General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'>
					<b>
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="MODIFIED" />
					</b>
				</label>
			</td>
		</tr>
		<tr class="post">
			<td valign="top" width="140">
				&nbsp;</td>
			<td valign="top" class="message">
				<%# General.EncodeMessage(((System.Data.DataRowView)Container.DataItem)["Message"].ToString())%>
			</td>
		</tr>
		<tr class="postfooter">
			<td class="small">
				<a href="javascript:scroll(0,0)">
					<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="TOP" />
				</a>
			</td>
			<td class="postfooter">
				<asp:LinkButton ID="ViewBtn" runat="server" Text='<%# GetText("VIEW") %>' CommandName="View"
					CommandArgument='<%# Eval("MessageID") %>' />&nbsp;
				<asp:LinkButton ID="CopyOverBtn" runat="server" Text='<%# GetText("COPYOVER") %>'
					CommandName="CopyOver" Visible='<%# General.CompareMessage(DataBinder.Eval(Container.DataItem, "[\"OriginalMessage\"]"),DataBinder.Eval(Container.DataItem, "[\"Message\"]"))%>'
					CommandArgument='<%# Eval("MessageID") %>' />&nbsp;
				<asp:LinkButton ID="ResolveBtn" runat="server" Text='<%# GetText("RESOLVED") %>'
					CommandName="Resolved" CommandArgument='<%# Eval("MessageID") %>' />&nbsp;
				<asp:LinkButton ID="DeleteBtn" runat="server" Text='<%# GetText("DELETE") %>' CommandName="Delete"
					CommandArgument='<%# Eval("MessageID") %>' OnLoad="Delete_Load" />&nbsp;
			</td>
		</tr>
	</ItemTemplate>
	<SeparatorTemplate>
		<tr class="postsep">
			<td colspan="2" style="height: 7px">
			</td>
		</tr>
	</SeparatorTemplate>
</asp:Repeater>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />

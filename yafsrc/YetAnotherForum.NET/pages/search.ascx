<%@ Control Language="c#" CodeFile="search.ascx.cs" AutoEventWireup="True" Inherits="YAF.Pages.search" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table class="content" cellspacing="1" cellpadding="0" width="100%">
	<tr>
		<td class="header1" colspan="2">
			<%= GetText("title") %>
		</td>
	</tr>
	<tr>
		<td class="postheader" colspan="2" align="center">
			<asp:DropDownList ID="listForum" runat="server" />
			<asp:DropDownList ID="listResInPage" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="postheader" width="35%" align="right">
			<%= GetText("postedby") %>
		</td>
		<td class="postheader" align="left">
			<asp:TextBox ID="txtSearchStringFromWho" runat="server" Width="350px" />
			<asp:DropDownList ID="listSearchFromWho" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="postheader" width="35%" align="right">
			<%=GetText("posts") %>
		</td>
		<td class="postheader" align="left">
			<asp:TextBox ID="txtSearchStringWhat" runat="server" Width="350px" />
			<asp:DropDownList ID="listSearchWhat" runat="server" />
		</td>
	</tr>
	<tr>
		<td class="postheader" colspan="2" align="center">
			<asp:Button ID="btnSearch" CssClass="pbutton" runat="server" OnClick="btnSearch_Click" />
		</td>
	</tr>
</table>
<br />

<YAF:Pager runat="server" ID="Pager" />

<table class="content" cellspacing="1" cellpadding="0" width="100%">
	<asp:Repeater ID="SearchRes" runat="server">
		<HeaderTemplate>
			<tr>
				<td class="header1" colspan="2">
					<%= GetText("RESULTS") %>
				</td>
			</tr>
		</HeaderTemplate>
		<ItemTemplate>
			<tr class="header2">
				<td colspan="2">
					<b>
						<%= GetText("topic") %>
					</b><a href="<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}",DataBinder.Eval(Container.DataItem, "TopicID")) %>">
						<%# DataBinder.Eval(Container.DataItem, "Topic") %>
					</a>
				</td>
			</tr>
			<tr class="postheader">
				<td width="140px" id="NameCell" valign="top">
					<a name="<%# DataBinder.Eval(Container.DataItem, "MessageID") %>" /><b><a href="<%# YafBuildLink.GetLink(ForumPages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>">
						<%# HtmlEncode(DataBinder.Eval(Container.DataItem, "Name")) %>
					</a></b>
				</td>
				<td width="80%" class="postheader">
					<b>
						<%# GetText("POSTED") %>
					</b>
					<%# YafDateTime.FormatDateTime( ( System.DateTime ) DataBinder.Eval( Container.DataItem, "Posted" ) )%>
				</td>
			</tr>
			<tr class="post">
				<td width="140px">
					&nbsp;</td>
				<td width="80%">
					<%# FormatMessage(Container.DataItem) %>
				</td>
			</tr>
		</ItemTemplate>
		<AlternatingItemTemplate>
			<tr class="header2">
				<td colspan="2">
					<b>
						<%= GetText("topic") %>
					</b><a href="<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}",DataBinder.Eval(Container.DataItem, "TopicID")) %>">
						<%# DataBinder.Eval(Container.DataItem, "Topic") %>
					</a>
				</td>
			</tr>
			<tr class="postheader">
				<td width="140px" id="NameCell" valign="top">
					<a name="<%# DataBinder.Eval(Container.DataItem, "MessageID") %>" /><b><a href="<%# YafBuildLink.GetLink(ForumPages.profile,"u={0}",DataBinder.Eval(Container.DataItem, "UserID")) %>">
						<%# HtmlEncode(Convert.ToString(DataBinder.Eval(Container.DataItem, "Name"))) %>
					</a></b>
				</td>
				<td width="80%" class="postheader">
					<b>
						<%# GetText("POSTED") %>
					</b>
					<%# YafDateTime.FormatDateTime( ( System.DateTime ) DataBinder.Eval( Container.DataItem, "Posted" ) )%>
				</td>
			</tr>
			<tr class="post_alt">
				<td width="140px">
					&nbsp;</td>
				<td width="80%">
					<%# FormatMessage(Container.DataItem) %>
				</td>
			</tr>
		</AlternatingItemTemplate>
		<FooterTemplate>
			<tr>
				<td class="footer1" colspan="2">
					&nbsp;</td>
			</tr>
		</FooterTemplate>
	</asp:Repeater>
	<asp:PlaceHolder ID="NoResults" runat="Server" Visible="false">
		<tr>
			<td class="header2" colspan="2">
				<%= GetText("RESULTS") %>
			</td>
		</tr>
		<tr>
			<td class="postheader" colspan="2" align="center">
				<br />
				<%= GetText("NO_SEARCH_RESULTS") %>
				<br />
			</td>
		</tr>
		<tr>
			<td class="footer1" colspan="2">
				&nbsp;</td>
		</tr>
	</asp:PlaceHolder>
</table>

<YAF:Pager ID="Pager1" runat="server" LinkedPager="Pager" />

<YAF:SmartScroller ID="SmartScroller1" runat="server" />

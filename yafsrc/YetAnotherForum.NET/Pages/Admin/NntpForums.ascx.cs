﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Admin;

using YAF.Types.Models;

/// <summary>
/// Admin Page to Edit NNTP Forums
/// </summary>
public partial class NntpForums : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NntpForums"/> class. 
    /// </summary>
    public NntpForums()
        : base("ADMIN_NNTPFORUMS", ForumPages.Admin_NntpForums)
    {
    }

    /// <summary>
    /// News the forum click.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void NewForumClick(object sender, EventArgs e)
    {
        this.EditDialog.BindData(null);

        this.PageBoardContext.PageElements.RegisterJsBlockStartup(
            "openModalJs",
            JavaScriptBlocks.OpenModalJs("NntpForumEditDialog"));
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!this.IsPostBack)
        {
            this.BindData();
        }
    }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddRoot();

        this.PageBoardContext.PageLinks.AddAdminIndex();
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_NNTPFORUMS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Ranks the list item command.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="e">The <see cref="RepeaterCommandEventArgs"/> instance containing the event data.</param>
    protected void RankListItemCommand(object source, RepeaterCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "edit":
                this.EditDialog.BindData(e.CommandArgument.ToType<int>());

                this.PageBoardContext.PageElements.RegisterJsBlockStartup(
                    "openModalJs",
                    JavaScriptBlocks.OpenModalJs("NntpForumEditDialog"));
                break;
            case "delete":
                var forumId = e.CommandArgument.ToType<int>();

                this.GetRepository<NntpTopic>().Delete(n => n.NntpForumID == forumId);
                this.GetRepository<NntpForum>().Delete(n => n.ID == forumId);

                this.BindData();
                break;
        }
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        this.RankList.DataSource = this.GetRepository<NntpForum>()
            .NntpForumList(this.PageBoardContext.PageBoardID, null);
        this.DataBind();

        if (this.RankList.Items.Count == 0)
        {
            this.EmptyState.Visible = true;
        }
    }
}
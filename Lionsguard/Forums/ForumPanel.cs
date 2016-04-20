using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Lionsguard.Security;

namespace Lionsguard.Forums
{
	[ToolboxData("<{0}:ForumPanel runat=server></{0}:ForumPanel>")]
	public class ForumPanel : WebControl, INamingContainer
	{
		#region Static Properties
		public static readonly string NewTopicCommandName = "NewTopic";
		public static readonly string NewReplyCommandName = "NewReply";
		public static readonly string SaveCommandName = "Save";
		public static readonly string CancelCommandName = "Cancel";

		public static readonly string MceClientScriptKey = "jsMce";
		public static readonly string MceScript = @"tinyMCE.init({0}
	mode : 'exact',
	elements : '{1}',
	theme : 'advanced',
	plugins : 'emotions',
	theme_advanced_buttons1 : 'bold,italic,underline,separator,strikethrough,justifyleft,justifycenter,justifyright,justifyfull,bullist,numlist,undo,redo,link,unlink,emotions',
	theme_advanced_buttons2 : '',
	theme_advanced_buttons3 : '',
	theme_advanced_toolbar_location : 'top',
	theme_advanced_toolbar_align : 'left',
	theme_advanced_statusbar_location : 'bottom',
	extended_valid_elements : 'a[name|href|target|title|onclick],img[class|src|border=0|alt|title|hspace|vspace|width|height|align|onmouseover|onmouseout|name],hr[class|width|size|noshade],font[face|size|color|style],span[class|align|style]'
{2});";
		#endregion

		#region Properties
		private Style _breadcrumbsStyle = null;
		private Style _rssLinkStyle = null;
		private TableStyle _contentStyle = null;
		private TableStyle _addContentStyle = null;
		private Style _alternateColumnStyle = null;
		private Style _alertStyleSuccess = null;
		private Style _alertStyleError = null;

		private string AlertMessage { get; set; }
		private ForumAlertType AlertType { get; set; }

		private string Title { get; set; }
		private string Content { get; set; }	

		[Bindable(true)]
		[Category("Style")]
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The style applied to the breadcrumbs links.")]
		public virtual Style BreadcrumbsStyle
		{
			get
			{
				if (_breadcrumbsStyle == null)
				{
					_breadcrumbsStyle = new Style();
					if (base.IsTrackingViewState)
					{
						((IStateManager)_breadcrumbsStyle).TrackViewState();
					}
				}
				return _breadcrumbsStyle;
			}
		}

		[Bindable(true)]
		[Category("Style")]
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The style applied to success alert messages.")]
		public virtual Style AlertStyleSuccess
		{
			get
			{
				if (_alertStyleSuccess == null)
				{
					_alertStyleSuccess = new Style();
					if (base.IsTrackingViewState)
					{
						((IStateManager)_alertStyleSuccess).TrackViewState();
					}
				}
				return _alertStyleSuccess;
			}
		}

		[Bindable(true)]
		[Category("Style")]
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The style applied to error alert messages.")]
		public virtual Style AlertStyleError
		{
			get
			{
				if (_alertStyleError == null)
				{
					_alertStyleError = new Style();
					if (base.IsTrackingViewState)
					{
						((IStateManager)_alertStyleError).TrackViewState();
					}
				}
				return _alertStyleError;
			}
		}

		[Bindable(true)]
		[Category("Style")]
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The style applied to the RSS link.")]
		public virtual Style RssLinkStyle
		{
			get
			{
				if (_rssLinkStyle == null)
				{
					_rssLinkStyle = new Style();
					if (base.IsTrackingViewState)
					{
						((IStateManager)_rssLinkStyle).TrackViewState();
					}
				}
				return _rssLinkStyle;
			}
		}

		[Bindable(true)]
		[Category("Style")]
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The style applied to alternate columns of the grid.")]
		public virtual Style AlternateColumnStyle
		{
			get
			{
				if (_alternateColumnStyle == null)
				{
					_alternateColumnStyle = new Style();
					if (base.IsTrackingViewState)
					{
						((IStateManager)_alternateColumnStyle).TrackViewState();
					}
				}
				return _alternateColumnStyle;
			}
		}

		[Bindable(true)]
		[Category("Style")]
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The style applied to the Content table.")]
		public virtual TableStyle ContentStyle
		{
			get
			{
				if (_contentStyle == null)
				{
					_contentStyle = new TableStyle();
					if (base.IsTrackingViewState)
					{
						((IStateManager)_contentStyle).TrackViewState();
					}
				}
				return _contentStyle;
			}
		}

		[Bindable(true)]
		[Category("Style")]
		[DefaultValue((string)null)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Description("The style applied to the Add Content table.")]
		public virtual TableStyle AddContentStyle
		{
			get
			{
				if (_addContentStyle == null)
				{
					_addContentStyle = new TableStyle();
					if (base.IsTrackingViewState)
					{
						((IStateManager)_addContentStyle).TrackViewState();
					}
				}
				return _addContentStyle;
			}
		}

		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("0")]
		[Localizable(true)]
		public bool EnableBoardIndexLink
		{
			get
			{
				object obj = ViewState["EnableBoardIndexLink"];
				if (obj != null)
				{
					return (bool)obj;
				}
				return true;
			}
			set { ViewState["EnableBoardIndexLink"] = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("0")]
		[Localizable(true)]
		public int BoardID
		{
			get
			{
				object obj = ViewState["BoardID"];
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
			set { ViewState["BoardID"] = value; }
		}

		private int ForumID
		{
			get
			{
				object obj = ViewState["ForumID"];
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
			set { ViewState["ForumID"] = value; }
		}

		private int TopicID
		{
			get
			{
				object obj = ViewState["TopicID"];
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
			set { ViewState["TopicID"] = value; }
		}

		private int StartIndex
		{
			get
			{
				object obj = ViewState["StartIndex"];
				if (obj != null)
				{
					return (int)obj;
				}
				return 0;
			}
			set { ViewState["StartIndex"] = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("0")]
		[Localizable(true)]
		public string ForumRootUrl
		{
			get
			{
				object obj = ViewState["ForumRootUrl"];
				if (obj != null)
				{
					return (string)obj;
				}
				return "/";
			}
			set  { ViewState["ForumRootUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		public string RssIconUrl
		{
			get
			{
				object obj = ViewState["RssIconUrl"];
				if (obj != null)
				{
					return (string)obj;
				}
				return String.Empty;
			}
			set { ViewState["RssIconUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Data")]
		[DefaultValue("")]
		[Localizable(true)]
		public string RssHandlerUrl
		{
			get
			{
				object obj = ViewState["RssHandlerUrl"];
				if (obj != null)
				{
					return (string)obj;
				}
				return String.Empty;
			}
			set { ViewState["RssHandlerUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public string TopicButtonImageUrl
		{
			get
			{
				object obj = ViewState["TopicButtonImageUrl"];
				if (obj != null)
				{
					return (string)obj;
				}
				return String.Empty;
			}
			set { ViewState["TopicButtonImageUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public ButtonType TopicButtonType
		{
			get
			{
				object obj = ViewState["TopicButtonType"];
				if (obj != null)
				{
					return (ButtonType)obj;
				}
				return ButtonType.Button;
			}
			set { ViewState["TopicButtonType"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public string ReplyButtonImageUrl
		{
			get
			{
				object obj = ViewState["ReplyButtonImageUrl"];
				if (obj != null)
				{
					return (string)obj;
				}
				return String.Empty;
			}
			set { ViewState["ReplyButtonImageUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public ButtonType ReplyButtonType
		{
			get
			{
				object obj = ViewState["ReplyButtonType"];
				if (obj != null)
				{
					return (ButtonType)obj;
				}
				return ButtonType.Button;
			}
			set { ViewState["ReplyButtonType"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public string SaveButtonImageUrl
		{
			get
			{
				object obj = ViewState["SaveButtonImageUrl"];
				if (obj != null)
				{
					return (string)obj;
				}
				return String.Empty;
			}
			set { ViewState["SaveButtonImageUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public ButtonType SaveButtonType
		{
			get
			{
				object obj = ViewState["SaveButtonType"];
				if (obj != null)
				{
					return (ButtonType)obj;
				}
				return ButtonType.Button;
			}
			set { ViewState["SaveButtonType"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public string CancelButtonImageUrl
		{
			get
			{
				object obj = ViewState["CancelButtonImageUrl"];
				if (obj != null)
				{
					return (string)obj;
				}
				return String.Empty;
			}
			set { ViewState["CancelButtonImageUrl"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public ButtonType CancelButtonType
		{
			get
			{
				object obj = ViewState["CancelButtonType"];
				if (obj != null)
				{
					return (ButtonType)obj;
				}
				return ButtonType.Button;
			}
			set { ViewState["CancelButtonType"] = value; }
		}

		[Bindable(true)]
		[Category("Appearence")]
		[DefaultValue("")]
		[Localizable(true)]
		public string TinyMceScriptPath
		{
			get
			{
				object obj = ViewState["TinyMceScriptPath"];
				if (obj != null)
				{
					return (string)obj;
				}
				return String.Empty;
			}
			set { ViewState["TinyMceScriptPath"] = value; }
		}

		protected ForumState State
		{
			get
			{
				object obj = ViewState["State"];
				if (obj != null)
				{
					return (ForumState)obj;
				}
				return ForumState.AllBoards;
			}
			set { ViewState["State"] = value; }
		}
		#endregion

		#region OnLoad
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!this.Page.IsPostBack)
			{
				string start = this.Page.Request.QueryString["start"];
				string b = this.Page.Request.QueryString["b"];
				string f = this.Page.Request.QueryString["f"];
				string t = this.Page.Request.QueryString["t"];
				string a = this.Page.Request.QueryString["a"];
				string msg = this.Page.Request.QueryString["msg"];

				int result;
				if (Int32.TryParse(start, out result)) this.StartIndex = result;
				else this.StartIndex = 0;
				if (Int32.TryParse(b, out result))
				{
					this.BoardID = result;
				}
				if (this.BoardID > 0)
				{
					this.State = ForumState.ViewBoard;
				}
				if (Int32.TryParse(f, out result))
				{
					this.ForumID = result;
					this.State = ForumState.ViewForum;
				}
				if (Int32.TryParse(t, out result))
				{
					this.TopicID = result;
					this.State = ForumState.ViewTopic;
				}
				bool add = false;
				if (Boolean.TryParse(a, out add))
				{
					if (this.ForumID > 0)
					{
						this.State = ForumState.AddTopic;
					}
					if (this.TopicID > 0)
					{
						this.State = ForumState.AddReply;
					}
				}

				if (!String.IsNullOrEmpty(msg))
				{
					this.SetAlert(ForumAlertType.Success, HttpUtility.UrlDecode(msg));
				}
			}
		}
		#endregion

		#region OnBubbleEvent
		protected override bool OnBubbleEvent(object source, EventArgs e)
		{
			bool handled = false;
			if (e is CommandEventArgs)
			{
				CommandEventArgs args = (CommandEventArgs)e;
				if (args.CommandName.Equals(NewTopicCommandName, StringComparison.InvariantCultureIgnoreCase))
				{
					this.AttemptNewTopic();
					handled = true;
				}
				else if (args.CommandName.Equals(NewReplyCommandName, StringComparison.InvariantCultureIgnoreCase))
				{
					this.AttemptNewReply();
					handled = true;
				}
				else if (args.CommandName.Equals(SaveCommandName, StringComparison.InvariantCultureIgnoreCase))
				{
					this.AttemptSave();
					handled = true;
				}
				else if (args.CommandName.Equals(CancelCommandName, StringComparison.InvariantCultureIgnoreCase))
				{
					this.AttemptCancel();
					handled = true;
				}
			}
			return handled;
		}
		#endregion

		#region CreateChildControls
		protected override void CreateChildControls()
		{
			this.Controls.Clear();

			if (this.DesignMode) return;
			//=====================================================================================
			// LAYOUT
			//=====================================================================================
			IEnumerable dataSource = null;
			Link[] links = null;
			string[] columnNames = null;
			string title = String.Empty;
			switch (this.State)
			{
				case ForumState.ViewBoard:
					Board board = ForumManager.GetBoard(this.BoardID);
					if (board != null)
					{
						title = board.Name;
						dataSource = ForumManager.GetForums(this.BoardID);
						links = new Link[] 
							{ 
								new Link { Text = board.Name, Url = String.Format("{0}?b={1}", this.ForumRootUrl, board.ID) }
							};
						columnNames = new string[] { "Forums", "Topics", "Replies", "Last Post" };
					}
					break;
				case ForumState.ViewForum:
				case ForumState.AddTopic:
					Forum forum = ForumManager.GetForum(this.ForumID);
					if (forum != null)
					{
						title = forum.Title;
						dataSource = ForumManager.GetTopics(this.ForumID, this.StartIndex, 10);
						links = new Link[] 
							{ 
								new Link { Text = forum.Board.Name, Url = String.Format("{0}?b={1}", this.ForumRootUrl, forum.Board.ID) },
								new Link { Text = forum.Title, Url = String.Format("{0}?f={1}", this.ForumRootUrl, forum.ID) }
							};
						columnNames = new string[] { "Topics", "Authors", "Replies", "Views", "Last Post" };
					}
					break;
				case ForumState.ViewTopic:
				case ForumState.AddReply:
					Topic topic = ForumManager.GetTopic(this.TopicID);
					if (topic != null)
					{
						ForumManager.UpdateTopicViewCount(this.TopicID);
						title = topic.Title;
						List<Post> replies = ForumManager.GetReplies(this.TopicID, this.StartIndex, 10).Cast<Post>().ToList();
						replies.Insert(0, topic as Post);
						dataSource = replies;
						links = new Link[]
							{
								new Link { Text = topic.Forum.Board.Name, Url = String.Format("{0}?b={1}", this.ForumRootUrl, topic.Forum.Board.ID) },
								new Link { Text = topic.Forum.Title, Url = String.Format("{0}?f={1}", this.ForumRootUrl, topic.Forum.ID) },
								new Link { Text = topic.Title, Url = String.Format("{0}?t={1}", this.ForumRootUrl, topic.ID) }
							};
						columnNames = new string[] { "Author", "Message" };
					}
					break;
				case ForumState.AllBoards:
					// AllBoards
					title = "Lionsguard Community";
					dataSource = ForumManager.GetBoards();
					links = null;
					columnNames = new string[] { "Boards" };
					break;
			}

			if (dataSource == null)
			{
				try
				{
					this.Page.Response.Redirect(this.ForumRootUrl);
				}
				catch (System.Threading.ThreadAbortException) { }
			}


			this.Page.Title = title;

			//-------------------------------------------------------------------------------------
			// Breacrumbs
			//-------------------------------------------------------------------------------------
			this.CreateBreadCrumbsControls(links);

			//-------------------------------------------------------------------------------------
			// Header
			//-------------------------------------------------------------------------------------
			this.Controls.Add(new LiteralControl(String.Format("<h1>{0}</h1>", title)));

			//-------------------------------------------------------------------------------------
			// RSS
			//-------------------------------------------------------------------------------------
			if (!String.IsNullOrEmpty(this.RssHandlerUrl))
			{
				HyperLink rssLink = new HyperLink();
				rssLink.ApplyStyle(this.RssLinkStyle);
				rssLink.NavigateUrl = this.RssHandlerUrl;
				if (!String.IsNullOrEmpty(this.RssIconUrl))
				{
					rssLink.ImageUrl = this.RssIconUrl;
				}
				this.Controls.Add(rssLink);
			}

			//-------------------------------------------------------------------------------------
			// Alerts
			//-------------------------------------------------------------------------------------
			if (!String.IsNullOrEmpty(this.AlertMessage))
			{
				Panel pnlAlerts = new Panel();
				switch (this.AlertType)
				{
					case ForumAlertType.Success:
						pnlAlerts.ApplyStyle(this.AlertStyleSuccess);
						break;
					case ForumAlertType.Error:
						pnlAlerts.ApplyStyle(this.AlertStyleError);
						break;
				}
				pnlAlerts.Controls.Add(new LiteralControl(this.AlertMessage));
				this.Controls.Add(pnlAlerts);
			}

			//-------------------------------------------------------------------------------------
			// CONTENT
			//-------------------------------------------------------------------------------------
			this.CreateContentControls(dataSource, columnNames);

			// TODO: Send PM Link, RSS Link, Delicious, Kick

			//-------------------------------------------------------------------------------------
			// Stats/Legend
			//=====================================================================================
		}
		#endregion

		#region CreateBreadCrumbsControls
		private void CreateBreadCrumbsControls(params Link[] links)
		{
			Panel pnl = new Panel();
			pnl.ApplyStyle(this.BreadcrumbsStyle);
			this.Controls.Add(pnl);

			bool writeSpacer = this.EnableBoardIndexLink;
			if (this.EnableBoardIndexLink)
			{
				HyperLink lnk = new HyperLink();
				lnk.NavigateUrl = this.ForumRootUrl;
				lnk.Text = "Board Index";
				pnl.Controls.Add(lnk);
			}

			if (links != null && links.Length > 0)
			{
				foreach (var link in links)
				{
					if (writeSpacer) pnl.Controls.Add(new LiteralControl(" &#187; "));
					HyperLink hl = new HyperLink();
					hl.NavigateUrl = link.Url;
					hl.Text = link.Text;
					pnl.Controls.Add(hl);
					writeSpacer = true;
				}
			}
		}
		#endregion

		#region CreateButtonControls
		private void CreateButtonControls()
		{
			Panel pnlButtons = new Panel();
			pnlButtons.Style.Add(HtmlTextWriterStyle.Padding, "4px");
			switch (this.State)
			{
				case ForumState.ViewForum:
					pnlButtons.Controls.Add(this.CreateButton(this.TopicButtonType, "New Topic", this.TopicButtonImageUrl, NewTopicCommandName));
					this.Controls.Add(pnlButtons);
					break;
				case ForumState.ViewTopic:
					pnlButtons.Controls.Add(this.CreateButton(this.TopicButtonType, "New Topic", this.TopicButtonImageUrl, NewTopicCommandName));
					pnlButtons.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
					pnlButtons.Controls.Add(this.CreateButton(this.ReplyButtonType, "New Reply", this.ReplyButtonImageUrl, NewReplyCommandName));
					this.Controls.Add(pnlButtons);
					break;
			}
		}
		#endregion

		#region CreateContentControls
		private void CreateContentControls(IEnumerable dataSource, string[] columnNames)
		{
			if (this.State == ForumState.AddTopic || this.State == ForumState.AddReply)
			{
				// <table cellpadding="4" cellspacing="0" border="0" width="100%" class="container">
				Table tblAdd = new Table();
				tblAdd.CellPadding = 4;
				tblAdd.CellSpacing = 0;
				tblAdd.Width = Unit.Percentage(100);
				tblAdd.ApplyStyle(this.AddContentStyle);
				this.Controls.Add(tblAdd);

				TableRow tr = null;
				TableCell td = null;
				if (this.State == ForumState.AddTopic)
				{
					tr = new TableRow();
					tblAdd.Rows.Add(tr);

					td = new TableCell();
					tr.Cells.Add(td);
					td.ColumnSpan = 2;
					td.Text = "Post a new topic";

					tr = new TableRow();
					tblAdd.Rows.Add(tr);

					td = new TableCell();
					tr.Cells.Add(td);
					td.Font.Bold = true;
					td.Text = "Title:";

					td = new TableCell();
					tr.Cells.Add(td);
					TextBox txtTitle = new TextBox();
					txtTitle.MaxLength = 128;
					txtTitle.Width = Unit.Pixel(200);
					txtTitle.TextChanged += new EventHandler(TitleTextChanged);
					td.Controls.Add(txtTitle);
				}
				else
				{
					tr = new TableRow();
					tblAdd.Rows.Add(tr);

					td = new TableCell();
					tr.Cells.Add(td);
					td.ColumnSpan = 2;
					td.Text = "Post a new reply";
				}

				tr = new TableRow();
				tblAdd.Rows.Add(tr);

				td = new TableCell();
				tr.Cells.Add(td);
				td.Font.Bold = true;
				td.Text = "Message:";
				td.VerticalAlign = VerticalAlign.Top;

				td = new TableCell();
				tr.Cells.Add(td);
				TextBox txtContent = new TextBox();
				txtContent.ID = "ContentTextBox";
				txtContent.MaxLength = 8000;
				txtContent.Width = Unit.Percentage(90);
				txtContent.Height = Unit.Pixel(350);
				txtContent.TextMode = TextBoxMode.MultiLine;
				txtContent.TextChanged += new EventHandler(ContentTextChanged);
				td.Controls.Add(txtContent);

				// MCE
				if (!this.Page.ClientScript.IsClientScriptBlockRegistered(MceClientScriptKey))
				{
					this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), 
						MceClientScriptKey, String.Format(MceScript, "{", txtContent.ClientID, "}"), true);
				}

				// Buttons
				tr = new TableRow();
				tblAdd.Rows.Add(tr);

				td = new TableCell();
				tr.Cells.Add(td);
				td.Text = String.Empty;

				td = new TableCell();
				tr.Cells.Add(td);
				td.Controls.Add(this.CreateButton(this.SaveButtonType, "Save", this.SaveButtonImageUrl, SaveCommandName, true));
				td.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
				td.Controls.Add(this.CreateButton(this.CancelButtonType, "Cancel", this.CancelButtonImageUrl, CancelCommandName));
			}
			else
			{
				//-------------------------------------------------------------------------------------
				// Buttons (New Topic, New Reply)
				//-------------------------------------------------------------------------------------
				this.CreateButtonControls();

				// <table class="forum" cellspacing="0" cellpadding="4" rules="all" border="1" style="width:100%;border-collapse:collapse;"> 
				Table tbl = new Table();
				this.Controls.Add(tbl);
				tbl.ApplyStyle(this.ContentStyle);
				tbl.CellPadding = 4;
				tbl.CellSpacing = 0;
				tbl.Width = Unit.Percentage(100);
				tbl.Attributes.Add("rules", "all");

				// TH
				TableRow trHeader = new TableRow();
				tbl.Rows.Add(trHeader);
				foreach (var name in columnNames)
				{
					TableHeaderCell th = new TableHeaderCell();
					trHeader.Cells.Add(th);
					th.Controls.Add(new LiteralControl(name));
				}

				int itemCount = (dataSource as ICollection).Count;
				foreach (var item in dataSource)
				{
					// <tr>
					TableRow tr = new TableRow();
					tbl.Rows.Add(tr);

					for (int i = 0; i < columnNames.Length; i++)
					{
						this.AddContentCell(tr, item, i, itemCount);
					}
				}

				//-------------------------------------------------------------------------------------
				// Buttons (New Topic, New Reply)
				//-------------------------------------------------------------------------------------
				this.CreateButtonControls();
			}
		}

		protected void TitleTextChanged(object sender, EventArgs e)
		{
			this.Title = (sender as TextBox).Text;
		}

		protected void ContentTextChanged(object sender, EventArgs e)
		{
			this.Content = (sender as TextBox).Text;
		}
		#endregion

		#region AddContentCell
		private void AddContentCell(TableRow row, object dataItem, int columnIndex, int itemCount)
		{
			TableCell td = new TableCell();
			row.Cells.Add(td);
			HyperLink lnk = null;
			HtmlGenericControl p = null;
			switch (this.State)
			{
				case ForumState.AllBoards:
					// Boards	
					Board board = (Board)dataItem;

					if (columnIndex == 0)
					{
						// [Boards]	
						lnk = new HyperLink();
						lnk.NavigateUrl = String.Format("{0}?b={1}", this.ForumRootUrl, board.ID);
						lnk.Text = board.Name;
						td.Controls.Add(lnk);
						td.Controls.Add(this.GetBreak());
						p = new HtmlGenericControl("p");
						p.Controls.Add(new LiteralControl(board.Description));
						td.Controls.Add(p);
					}
					if (columnIndex == 1)
					{
						// [Last Post]
					}
					break;
				case ForumState.ViewBoard:
					// Forums
					Forum forum = (Forum)dataItem;

					if (columnIndex == 0)
					{
						// [Forums]						
						lnk = new HyperLink();
						lnk.NavigateUrl = String.Format("{0}?f={1}", this.ForumRootUrl, forum.ID);
						lnk.Text = forum.Title;
						td.Controls.Add(lnk);
						td.Controls.Add(this.GetBreak());
						p = new HtmlGenericControl("p");
						p.Controls.Add(new LiteralControl(forum.Description));
						td.Controls.Add(p);
					}

					if (columnIndex == 1)
					{
						// [Topics]
						td.ApplyStyle(this.AlternateColumnStyle);
						td.HorizontalAlign = HorizontalAlign.Center;
						td.Width = Unit.Pixel(50);
						td.Controls.Add(new LiteralControl(forum.TopicCount.ToString()));
					}

					if (columnIndex == 2)
					{
						// [Replies]
						td.HorizontalAlign = HorizontalAlign.Center;
						td.Width = Unit.Pixel(50);
						td.Controls.Add(new LiteralControl(forum.ReplyCount.ToString()));
					}

					if (columnIndex == 3)
					{
						// [LastPost]
						td.ApplyStyle(this.AlternateColumnStyle);
						td.HorizontalAlign = HorizontalAlign.Center;
						td.Width = Unit.Pixel(150);
						if (!String.IsNullOrEmpty(forum.DisplayName))
						{
							td.Controls.Add(new LiteralControl(forum.LastPostDate.ToString()));
							td.Controls.Add(this.GetBreak());
							td.Controls.Add(new LiteralControl(forum.DisplayName));
						}
					}
					break;
				case ForumState.ViewForum:
					// Topics
					Topic topic = (Topic)dataItem;

					// [Topics]		
					if (columnIndex == 0)
					{
						lnk = new HyperLink();
						lnk.NavigateUrl = String.Format("{0}?t={1}", this.ForumRootUrl, topic.ID);
						lnk.Text = topic.Title;
						td.Controls.Add(lnk);
						td.Controls.Add(this.GetBreak());

						int pageCount = (topic.ReplyCount / 10);
						if (pageCount > 0)
						{
							td.Controls.Add(new LiteralControl("[ Go to page: "));
							for (int i = 0; i < pageCount; i++)
							{
								if (i > 0) td.Controls.Add(new LiteralControl(", "));
								lnk = new HyperLink();
								lnk.NavigateUrl = String.Format("{0}?t={1}&start={2}", this.ForumRootUrl, topic.ID, i);
								lnk.Text = (i + 1).ToString();
							}
							td.Controls.Add(new LiteralControl(" ]"));
						}
					}

					// [Author]		
					if (columnIndex == 1)
					{
						td.ApplyStyle(this.AlternateColumnStyle);
						td.HorizontalAlign = HorizontalAlign.Center;
						td.Width = Unit.Pixel(150);
						td.Controls.Add(new LiteralControl(topic.PostDate.ToString()));
						td.Controls.Add(this.GetBreak());
						td.Controls.Add(new LiteralControl(topic.DisplayName));
					}

					// [Replies]		
					if (columnIndex == 2)
					{
						td.HorizontalAlign = HorizontalAlign.Center;
						td.Width = Unit.Pixel(50);
						td.Controls.Add(new LiteralControl(topic.ReplyCount.ToString()));
					}

					// [Views]			
					if (columnIndex == 3)
					{
						td.ApplyStyle(this.AlternateColumnStyle);
						td.HorizontalAlign = HorizontalAlign.Center;
						td.Width = Unit.Pixel(50);
						td.Controls.Add(new LiteralControl(topic.ViewCount.ToString()));
					}

					// [LastPost]	
					if (columnIndex == 4)
					{
						td.HorizontalAlign = HorizontalAlign.Center;
						td.Width = Unit.Pixel(150);
						if (!String.IsNullOrEmpty(topic.LastAuthorDisplayName))
						{
							td.Controls.Add(new LiteralControl(topic.LastReplyDate.ToString()));
							td.Controls.Add(this.GetBreak());
							td.Controls.Add(new LiteralControl(topic.LastAuthorDisplayName));
						}
					}

					break;
				case ForumState.ViewTopic:
					// Replies (Post original topic and then list replies.)
					Post post = (Post)dataItem;

					// [Author]		
					// Author Name					
					// Image						
					// Date Posted		
					if (columnIndex == 0)
					{
						td.ApplyStyle(this.AlternateColumnStyle);
						td.HorizontalAlign = HorizontalAlign.Center;
						td.VerticalAlign = VerticalAlign.Top;
						td.Width = Unit.Pixel(150);
						HtmlGenericControl h3 = new HtmlGenericControl("h3");
						h3.Controls.Add(new LiteralControl(post.DisplayName));
						td.Controls.Add(h3);
						if (!String.IsNullOrEmpty(post.ImageUrl))
						{
							Image img = new Image();
							img.ImageUrl = post.ImageUrl;
							img.AlternateText = post.DisplayName;
							td.Controls.Add(img);
						}
						td.Controls.Add(this.GetBreak());
						td.Controls.Add(new LiteralControl(post.PostDate.ToString()));
					}

					// [Message]
					// Text
					// Author/Rankings
					if (columnIndex == 1)
					{
						td.VerticalAlign = VerticalAlign.Top;
						td.Controls.Add(new LiteralControl(post.Text));
						td.Controls.Add(this.GetBreak());
						HtmlGenericControl hr = new HtmlGenericControl("hr");
						td.Controls.Add(hr);
						Panel pnlContent = new Panel();
						pnlContent.Font.Bold = true;
						pnlContent.Controls.Add(new LiteralControl(post.DisplayName));
						td.Controls.Add(pnlContent);
						td.Controls.Add(this.GetBreak());
						td.Controls.Add(this.GetBreak());
					}
					break;
			}
		}
		#endregion

		#region GetBreak
		private Control GetBreak()
		{
			return new LiteralControl("<br/>");
		}
		#endregion

		#region CreateButton
		private Control CreateButton(ButtonType type, string text, string imageUrl, string commandName)
		{
			return this.CreateButton(type, text, imageUrl, commandName, false);
		}
		private Control CreateButton(ButtonType type, string text, string imageUrl, string commandName, bool causesValidation)
		{
			Control btn = null;
			switch (type)
			{	
				case ButtonType.Button:
					Button b = new Button();
					b.Text = text;
					b.CommandName = commandName;
					b.CausesValidation = causesValidation;
					btn = b;
					break;
				case ButtonType.Image:
					ImageButton i = new ImageButton();
					i.ImageUrl = imageUrl;
					i.AlternateText = i.ToolTip = text;
					i.CommandName = commandName;
					i.CausesValidation = causesValidation;
					btn = i;
					break;
				case ButtonType.Link:
					LinkButton l = new LinkButton();
					l.Text = text;
					l.CommandName = commandName;
					l.CausesValidation = causesValidation;
					btn = l;
					break;
			}
			return btn;
		}
		#endregion

		#region AttemptNewTopic
		private void AttemptNewTopic()
		{
			string url = String.Empty;
			User user = SecurityManager.GetUser(this.Context);
			if (user != null)
			{
				url = String.Format("{0}?f={1}&a=true", this.ForumRootUrl, this.ForumID);
			}
			else
			{
				url = String.Concat(Lionsguard.Settings.LoginUrl, "?ReturnUrl=", HttpUtility.UrlEncode(String.Format("{0}?f={1}&a=true", this.ForumRootUrl, this.ForumID)));
			}
			try
			{
				this.Page.Response.Redirect(url);
			}
			catch (System.Threading.ThreadAbortException) { }	
		}
		#endregion

		#region AttemptNewReply
		private void AttemptNewReply()
		{
			string url = String.Empty;
			User user = SecurityManager.GetUser(this.Context);
			if (user != null)
			{
				url = String.Format("{0}?t={1}&a=true", this.ForumRootUrl, this.TopicID);
			}
			else
			{
				url = String.Concat(Lionsguard.Settings.LoginUrl, "?ReturnUrl=", HttpUtility.UrlEncode(String.Format("{0}?t={1}&a=true", this.ForumRootUrl, this.TopicID)));
			}
			try
			{
				this.Page.Response.Redirect(url);
			}
			catch (System.Threading.ThreadAbortException) { }
		}
		#endregion

		#region AttemptSave
		private void AttemptSave()
		{
			if (this.State == ForumState.AddReply || this.State == ForumState.AddTopic)
			{
				if (this.State == ForumState.AddTopic)
				{
					if (String.IsNullOrEmpty(this.Title))
					{
						this.SetAlert(ForumAlertType.Error, "Title is required.");
						return;
					}
				}

				if (String.IsNullOrEmpty(this.Content))
				{
					this.SetAlert(ForumAlertType.Error, "Message is required.");
					return;
				}

				User user = SecurityManager.GetUser(this.Context);
				string message = String.Empty;
				string qsArg = "f";
				int id = 0;
				if (this.State == ForumState.AddTopic)
				{
					ForumManager.SaveTopic(new Topic { Title = this.Title, Text = this.Content, PostDate = DateTime.Now },
					   new Forum { ID = this.ForumID }, user.ID, this.Page.Request.UserHostAddress);

					qsArg = "f";
					id = this.ForumID;
					message = "Topic posted successfully!";
				}
				else
				{
					ForumManager.SaveReply(new Reply { Text = this.Content, PostDate = DateTime.Now },
						new Topic { ID = this.TopicID }, user.ID, this.Page.Request.UserHostAddress);
					
					qsArg = "t";
					id = this.TopicID;
					message = "Reply posted successfully!";
				}
				try
				{
					this.Page.Response.Redirect(String.Format("{0}?{1}={2}&msg={3}", 
						this.ForumRootUrl, qsArg, id, HttpUtility.UrlEncode(message)));
				}
				catch (System.Threading.ThreadAbortException) { }
			}
		}
		#endregion

		#region AttemptCancel
		private void AttemptCancel()
		{
			try
			{
				this.Page.Response.Redirect(String.Format("{0}?{1}={2}", 
					this.ForumRootUrl, 
					(this.State == ForumState.AddTopic ? "f" : "t"),
					(this.State == ForumState.AddTopic ? this.ForumID : this.TopicID)));
			}
			catch (System.Threading.ThreadAbortException) { }
		}
		#endregion

		#region SetAlert
		private void SetAlert(ForumAlertType type, string text)
		{
			this.AlertMessage = text;
			this.AlertType = type;
		}
		#endregion
	}

	public enum ForumState
	{
		AllBoards,
		ViewBoard,
		ViewForum,
		ViewTopic,
		AddTopic,
		AddReply,
	}

	public enum ForumAlertType
	{
		Success,
		Error
	}
}

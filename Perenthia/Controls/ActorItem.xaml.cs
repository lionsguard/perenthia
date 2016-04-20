using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Controls
{
	public partial class ActorItem : UserControl
	{
		#region Static Members
		public static ActorItem Create(RdlActor actorTag, bool displayItemsAsMerchandise)
		{
			ActorItem item = new ActorItem() 
							{ 
								ActorID = actorTag.ID, 
								ActorName = actorTag.Name 
							};

			// Each Actor has a list of actions associated with them, they are prefixed in the properties
			// collection with "Action_".
			List<RdlProperty> properties = actorTag.Properties.Where(p => p.Name.StartsWith("Action_")).ToList();
			foreach (var p in properties)
			{
				item.CreateButton(p.Name.Replace("Action_", ""));
			}
			return item;
		}
		#endregion

		#region Properties
		public int ActorID { get; set; }	

		public string ActorName
		{
			get { return (string)GetValue(ActorNameProperty); }
			set { SetValue(ActorNameProperty, value); }
		}
		public static readonly DependencyProperty ActorNameProperty = DependencyProperty.Register("ActorName", typeof(string), typeof(ActorItem), new PropertyMetadata(String.Empty, new PropertyChangedCallback(ActorItem.OnActorNamePropertyChanged)));
		private static void OnActorNamePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ActorItem).SetActorName();
		}
		#endregion

		#region Events
		public event ActionEventHandler Action = delegate { };
		#endregion

		public ActorItem()
		{
			InitializeComponent();
		}

		private void CreateButton(string name)
		{
			HyperlinkButton btn = new HyperlinkButton
									{
										Content = name,
										Cursor = Cursors.Hand,
										FontFamily = new FontFamily("Trebuchet"),
										FontSize = 10,
										Foreground = Brushes.LinkBrush,
										Tag = name
									};
			btn.Click += new RoutedEventHandler(ActionLinkClick);
			this.LinksPanel.Children.Add(btn);
		}

		private void ActionLinkClick(object sender, RoutedEventArgs e)
		{
			HyperlinkButton btn = sender as HyperlinkButton;
			if (btn != null)
			{
				string action = btn.Tag as String;
				if (!String.IsNullOrEmpty(action))
				{
					this.Action(this, new ActionEventArgs(action, this.ActorID, this.ActorName));
				}
			}
		}

		private void SetActorName()
		{
			this.NameElement.Text = this.ActorName;
		}
	}

	public delegate void ActionEventHandler(object sender, ActionEventArgs e);
	public class ActionEventArgs : EventArgs
	{
		public object ActorAlias { get; set; }
		public string ActorName { get; set; }	
		public string ActionName { get; set; }
		public List<object> Args { get; set; }
		public bool Cancel { get; set; }	

		public ActionEventArgs(string actionName, object actorId, string actorName)
		{
			this.ActionName = actionName;
			this.ActorAlias = actorId;
			this.ActorName = actorName;
			this.Args = new List<object>();
		}

		public ActionEventArgs(string actionName, object actorId, string actorName, params object[] args)
			: this(actionName, actorId, actorName)
		{
			this.Args.AddRange(args);
		}
	}
}

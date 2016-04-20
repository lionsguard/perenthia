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
using Radiance.Markup;
using Perenthia.Controls;
using Radiance;

namespace Perenthia.Windows
{
	public partial class ChatWindow : FloatableWindow, IWindow
	{
		public event ChatInputReceivedEventHandler ChatInputReceived = delegate { };
		public event ActionEventHandler Action = delegate { };	

		public Point3 Location
		{
			get { return (Point3)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		public static readonly DependencyProperty LocationProperty = DependencyProperty.Register("Location", typeof(Point3), typeof(ChatWindow), new PropertyMetadata(new Point3()));


		private bool _loaded = false;
		private RdlActorDictionary _actors = new RdlActorDictionary();

		public ChatWindow()
		{
			InitializeComponent();
			this.Loaded += (o, e) => { _loaded = true; };
		}

		private void CommandText_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				ChatInputReceived(new ChatInputReceivedEventArgs { Text = CommandText.Text });
				CommandText.Text = String.Empty;
				//Game.FocusState = FocusState.Main;
			}

			// Handle reply to last tell.
			if ((CommandText.Text.ToLower().Equals("/r ")
				|| CommandText.Text.ToLower().Equals("/reply "))
				&& !String.IsNullOrEmpty(Game.LastTellReceivedFrom))
			{
				CommandText.Text += String.Concat(Game.LastTellReceivedFrom, " ");
			}

			Game.ProcessInput = false;
		}

		private void CommandText_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
		{
			Game.ProcessInput = true;
		}

		private void CommandText_GotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			Game.FocusState = FocusState.Chat;
		}

		private void CommandText_LostFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			Game.FocusState = FocusState.Main;
		}

		public void SetPrompt()
		{
			this.Prompt.Text = String.Format(SR.PromptFormat,
				Game.Player.Body,
				Game.Player.BodyMax,
				Game.Player.Mind,
				Game.Player.MindMax,
				Game.Player.Experience);
		}

		public void SetCommandLine(string text)
		{
			CommandText.Text = text;
		}

		public void ActivateTab(TextPanelTab tab)
		{
			switch (tab)
			{	
				case TextPanelTab.Chat:
					ChatPanel.ScrollToEnd();
					break;
				case TextPanelTab.Room:
				case TextPanelTab.RoomDescription:
					break;
				case TextPanelTab.Actions:
				case TextPanelTab.General:
					GeneralPanel.ScrollToEnd();
					break;
				case TextPanelTab.Tells:
					TellsPanel.ScrollToEnd();
					break;
				default:
					break;
			}
		}

		public void AppendChat(TextType type, string text, object tag, RoutedEventHandler linkCallback)
		{
			ChatPanel.Append(type, text, tag, linkCallback);
			this.SetPrompt();
		}

		public void AppendGeneral(TextType type, string text, object tag, RoutedEventHandler linkCallback)
		{
			GeneralPanel.Append(type, text, tag, linkCallback);
			this.SetPrompt();
		}

		public void AppendTells(TextType type, string text, object tag, RoutedEventHandler linkCallback)
		{
			TellsPanel.Append(type, text, tag, linkCallback);
			this.SetPrompt();
		}

		public void SetRoom(string name, Point3 location, IEnumerable<RdlActor> actors)
		{
			this.Location = location;
			PlaceNameLabel.Text = name;

			foreach (var actor in actors)
			{
				if (_actors.ContainsKey(actor.ID))
					_actors[actor.ID] = actor;
				else
					_actors.Add(actor.ID, actor);
			}

			// Remove any actors not in the current location.
			var removes = _actors.Where(a => new Point3(a.Value.Properties.GetValue<int>("X"),
					a.Value.Properties.GetValue<int>("Y"),
					a.Value.Properties.GetValue<int>("Z")) != this.Location).Select(a => a.Value.ID).ToList();
			foreach (var id in removes)
			{
				_actors.Remove(id);
			}

			ctlItems.Children.Clear();
			foreach (var actor in _actors.Values)
			{
				ObjectType type = (ObjectType)Enum.Parse(typeof(ObjectType), actor.Properties.GetValue<string>("ObjectType"), true);
				if (type == ObjectType.Mobile || type == ObjectType.Player)
				{
					AvatarListItem avatarItem = AvatarListItem.Create(actor);
					avatarItem.Action += Action;
					ctlItems.Children.Add(avatarItem);
				}
				else if (type == ObjectType.Actor)
				{
					ItemListItem listItem = ItemListItem.Create(actor);
					listItem.Action += Action;
					ctlItems.Children.Add(listItem);
				}
			}
		}

		public void ClearRoom()
		{
			_actors.Clear();
			ctlItems.Children.Clear();
		}

		private void TabsControls_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (!_loaded)
				return;

			if (e.AddedItems == null || e.AddedItems.Count == 0)
				return;

			var tab = e.AddedItems[0] as TabItem;
			if (tab == null)
				return;

			if (tab.Tag == null)
				return;

			var textTab = (TextPanelTab)Enum.Parse(typeof(TextPanelTab), tab.Tag.ToString(), true);
			this.ActivateTab(textTab);
		}

		#region IWindow Members


		public event EventHandler Maximized = delegate { };

		public string WindowID { get; set; }

		public Point Position
		{
			get { return GetPosition(); }
			set { Canvas.SetLeft(this, value.X); Canvas.SetTop(this, value.Y); }
		}

		public Size Size
		{
			get { return new Size(this.ActualWidth, this.ActualHeight); }
			set { this.Width = value.Width; this.Height = value.Height; }
		}

		public void Minimize()
		{
			this.OnMinimized(EventArgs.Empty);
		}

		public void Maximize()
		{
			this.Maximized(this, EventArgs.Empty);
		}

		#endregion
	}

	public delegate void ChatInputReceivedEventHandler(ChatInputReceivedEventArgs e);
	public class ChatInputReceivedEventArgs : EventArgs
	{
		public string Text { get; set; }
	}
}


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

using Lionsguard;
using Radiance;
using Radiance.Markup;

namespace Perenthia.Controls
{
	public partial class Chat : UserControl
	{
		public enum MessageType
		{
			Say,
			Tell,
			System,
			Error,
			News,
			PlaceDesc,
			Positive,
			Negative,
			Emote,
			PlaceName,
			PlaceExits,
			PlaceActors,
			PlaceAvatars,
			Help,
			Level,
			Cast,
			Melee,
		}

		public enum CommandType
		{
			User,
			Player,
		}

		public CommandType ServerCommandType
		{
			get { return (CommandType)GetValue(ServerCommandTypeProperty); }
			set { SetValue(ServerCommandTypeProperty, value); }
		}
		public static readonly DependencyProperty ServerCommandTypeProperty = DependencyProperty.Register("ServerCommandType", typeof(CommandType), typeof(Chat), new PropertyMetadata(CommandType.Player));

		public bool HideCommandBar
		{
			get { return ctlChat.HideCommandBar; }
			set { ctlChat.HideCommandBar = value; }
		}
			

		public event ActionEventHandler TellLinkClick = delegate { };

		public Chat()
		{
			InitializeComponent();
		}

		public void SetFocus(string text)
		{
			ctlChat.SetFocus(text);
		}

		#region Chat Panel
		private void ctlChat_InputReceived(object sender, Lionsguard.InputReceivedEventArgs e)
		{
			RdlCommandParserErrorType errType = RdlCommandParserErrorType.None;
			RdlCommand cmd;
			if (RdlCommand.TryParse(e.Input, out cmd, out errType))
			{
				// Only echo say, shout, tell and emotes.
				this.EchoInput(cmd);

				if (this.ServerCommandType == CommandType.Player)
				{
					ServerManager.Instance.SendCommand(cmd);
				}
				else
				{
					ServerManager.Instance.SendUserCommand(cmd);
				}
			}
			else
			{
				switch (errType)
				{
					case RdlCommandParserErrorType.NoTargetForTell:
						this.Write(MessageType.Error, "TELL requires the name of the person you wish to send a message.");
						break;
					case RdlCommandParserErrorType.NoArgumentsSpecified:
						this.Write(MessageType.Error, String.Format("No arguments were specified for the {0} command.", cmd.TypeName));
						break;
					case RdlCommandParserErrorType.InvalidNumberOfArguments:
						this.Write(MessageType.Error, String.Format("An invalid number of arguments were specified for the {0} command.", cmd.TypeName));
						break;
				}
			}
		}
		private void OnChatTellLinkClick(object sender, RoutedEventArgs e)
		{
			HyperlinkButton lnk = sender as HyperlinkButton;
			if (lnk != null && lnk.Tag != null)
			{
				// Tag is the Alias of the tell recipient.
				this.TellLinkClick(ctlChat, new ActionEventArgs(Actions.Tell, lnk.Tag, lnk.Tag.ToString()));
				//ctlRoom_Action(this, new ActionEventArgs(Actions.Tell, lnk.Tag, lnk.Tag.ToString()));
			}
		}
		private void EchoInput(RdlCommand cmd)
		{
			switch (cmd.TypeName)
			{
				case "SAY":
					if (!String.IsNullOrEmpty(cmd.Text))
					{
						this.Write(MessageType.Say, String.Concat("You say: ", cmd.Text));
					}
					break;
				case "SHOUT":
					if (!String.IsNullOrEmpty(cmd.Text))
					{
						this.Write(MessageType.Say, String.Concat("You shout: ", cmd.Text.ToUpper()));
					}
					break;
				case "TELL":
					string msg = cmd.GetArg<string>(1);
					if (!String.IsNullOrEmpty(msg))
					{
						this.Write(MessageType.Tell, String.Format("You tell {0}: {1}", cmd.Args[0], msg));
					}
					break;
				case "EMOTE":
					// TODO: Echo Emotes...
					break;
			}
		}
		public void Write(MessageType type, string text)
		{
			switch (type)
			{
				case MessageType.Tell:
					ctlChat.Append(text, Brushes.MsgTellBrush, FontWeights.Normal, 11);
					break;
				case MessageType.System:
					ctlChat.Append(text, Brushes.MsgSystemBrush, FontWeights.Normal, 11);
					break;
				case MessageType.Error:
					ctlChat.Append(text, Brushes.MsgErrorBrush, FontWeights.Bold, 12);
					break;
				case MessageType.News:
					ctlChat.Append(text, Brushes.MsgNewsBrush, FontWeights.Normal, 11);
					break;
				case MessageType.PlaceDesc:
					ctlChat.Append(text, Brushes.MsgSystemPlaceDescBrush, FontWeights.Normal, 11);
					break;
				case MessageType.PlaceName:
					ctlChat.Append(text, Brushes.MsgSystemPlaceNameBrush, FontWeights.Bold, 11);
					break;
				case MessageType.PlaceExits:
					ctlChat.Append(text, Brushes.MsgSystemPlaceExitsBrush, FontWeights.Normal, 11);
					break;
				case MessageType.PlaceActors:
					ctlChat.Append(text, Brushes.MsgSystemPlaceActorsBrush, FontWeights.Normal, 11);
					break;
				case MessageType.PlaceAvatars:
					ctlChat.Append(text, Brushes.MsgSystemPlaceAvatarsBrush, FontWeights.Normal, 11);
					break;
				case MessageType.Positive:
					ctlChat.Append(text, Brushes.PositiveBrush, FontWeights.Normal, 11);
					break;
				case MessageType.Negative:
					ctlChat.Append(text, Brushes.NegativeBrush, FontWeights.Normal, 11);
					break;
				case MessageType.Emote:
					ctlChat.Append(text, Brushes.MsgEmoteBrush, FontWeights.Normal, 11);
					break;
				case MessageType.Help:
					ctlChat.Append(text, Brushes.MsgHelpBrush, FontWeights.Bold, 11);
					break;
				case MessageType.Level:
					ctlChat.Append(text, Brushes.MsgLevelBrush, FontWeights.Bold, 11);
					break;
				case MessageType.Cast:
					ctlChat.Append(text, Brushes.MsgCastBrush, FontWeights.Normal, 11);
					break;
				case MessageType.Melee:
					ctlChat.Append(text, Brushes.MsgMeleeBrush, FontWeights.Normal, 11);
					break;
				default:
					ctlChat.Append(text, Brushes.MsgSayBrush, FontWeights.Normal, 11);
					break;
			}
		}
		public void WriteMessages(RdlTagCollection tags)
		{
			List<RdlMessage> messages = tags.GetMessages();
			if (messages.Count > 0)
			{
				foreach (var msg in messages)
				{
					switch (msg.TypeName)
					{
						case "ERROR":
							this.Write(MessageType.Error, msg.Text);
							break;
						case "SYSTEM":
							RdlSystemMessage sysMsg = msg as RdlSystemMessage;
							if (sysMsg != null)
							{
								switch ((RdlSystemMessage.PriorityType)sysMsg.Priority)
								{
									case RdlSystemMessage.PriorityType.PlaceDescription:
										this.Write(MessageType.PlaceDesc, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.PlaceName:
										this.Write(MessageType.PlaceName, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.PlaceExits:
										this.Write(MessageType.PlaceExits, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.PlaceActors:
										this.Write(MessageType.PlaceActors, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.PlaceAvatars:
										this.Write(MessageType.PlaceAvatars, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Positive:
										this.Write(MessageType.Positive, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Negative:
										this.Write(MessageType.Negative, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Emote:
										this.Write(MessageType.Emote, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Help:
										this.Write(MessageType.Help, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Level:
										this.Write(MessageType.Level, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Cast:
										this.Write(MessageType.Cast, msg.Text);
										break;
									case RdlSystemMessage.PriorityType.Melee:
										this.Write(MessageType.Melee, msg.Text);
										break;
									default:
										this.Write(MessageType.System, msg.Text);
										break;
								}
							}
							break;
						case "NEWS":
							this.Write(MessageType.News, msg.Text);
							break;
						case "TELL":
							RdlTellMessage tellMsg = (RdlTellMessage)msg;
							ctlChat.Append(ctlChat.CreateLink(tellMsg.Text, Brushes.MsgTellBrush, FontWeights.Normal, 11, tellMsg.From, new RoutedEventHandler(this.OnChatTellLinkClick)));
							//this.Write(MessageType.Tell, msg.Text);
							break;
						default:
							this.Write(MessageType.Say, msg.Text);
							break;
					}
				}
			}
		}
		#endregion
	}
}

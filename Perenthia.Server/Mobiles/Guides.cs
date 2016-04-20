using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia.Mobiles
{
	#region Guide
	public abstract class Guide : Npc
	{
		public abstract bool CanDisplayMessage(Avatar avatar);
		public abstract string GetDisplayMessage(Avatar avatar);
		public abstract void MessageDisplayed(Avatar avatar);

		public override void OnEnter(Actor sender, IMessageContext context, Direction direction)
		{	
			Avatar avatar = sender as Avatar;
			if (avatar != null)
			{
				if (this.CanDisplayMessage(avatar))
				{
					context.Add(new RdlChatMessage(this.Name, String.Concat(this.Name, " says: ", this.GetDisplayMessage(avatar))));

					this.MessageDisplayed(avatar);
				}
			}
		}
	}
	#endregion

	#region StartGuide
	public class StartGuide : Guide
	{
		public string IntroMessage
		{	
			get { return this.Properties.GetValue<string>(IntroMessageProperty); }
			set { this.Properties.SetValue(IntroMessageProperty, value); }
		}
		public static readonly string IntroMessageProperty = "IntroMessage";

		public StartGuide()
		{
			this.IntroMessage = String.Empty;
			this.RdlIgnoreProperties.Add(IntroMessageProperty);
		}

		public virtual string HasViewedKeyName
		{
			get { return "HasViewedStartGuideMessage"; }
		}

		public override string GetDisplayMessage(Avatar avatar)
		{
			return String.Format(this.IntroMessage, avatar.Name);
		}

		public override bool CanDisplayMessage(Avatar avatar)
		{
			return !avatar.Properties.GetValue<bool>(HasViewedKeyName);
		}

		public override void MessageDisplayed(Avatar avatar)
		{
			// Set the viewed property so the message will not display again.
			avatar.Properties.SetValue(HasViewedKeyName, true);
		}
	}
	#endregion

	#region TravelGuide
	public class TravelGuide : Guide
	{
		public string Message
		{	
			get { return this.Properties.GetValue<string>(MessageProperty); }
			set { this.Properties.SetValue(MessageProperty, value); }
		}
		public static readonly string MessageProperty = "Message";

		/// <summary>
		/// Gets or sets a value indicating whether or not the cost of the travel is in emblem or gold. 
		/// True for emblem and false for gold.
		/// </summary>
		public bool IsCostInEmblem
		{	
			get { return this.Properties.GetValue<bool>(IsCostInEmblemProperty); }
			set { this.Properties.SetValue(IsCostInEmblemProperty, value); }
		}
		public static readonly string IsCostInEmblemProperty = "IsCostInEmblem";

		public TravelDetailCollection TravelDetails { get; private set; }	

		public TravelGuide()
		{
			this.TravelDetails = new TravelDetailCollection(this);
			this.IsCostInEmblem = true;

			this.Message = String.Empty;
		}

		public override string GetDisplayMessage(Avatar avatar)
		{
			return this.Message;
		}

		public override bool CanDisplayMessage(Avatar avatar)
		{
			return true;
		}

		public override void MessageDisplayed(Avatar avatar)
		{
		}
	}
	public class TravelDetailCollection : ActorOwnedDictionaryBase<int>
	{
		public TravelDetailCollection(Actor owner)
			: base(owner, "TravelTo_")
		{
		}

		public override bool TryGetValue(string key, out int value)
		{
			value = 0;
			if (this.Owner.Properties.ContainsKey(this.GetPrefixedName(key)))
			{
				value = this.Owner.Properties.GetValue<int>(this.GetPrefixedName(key));
				return true;
			}
			return false;
		}
	}
	#endregion
}

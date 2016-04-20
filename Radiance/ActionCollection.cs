using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	/// <summary>
	/// Represents static properties that define standard actions.
	/// </summary>
	public static class Actions
	{
		public const string Tell = "Tell";
		public const string Attack = "Attack";
		public const string Invite = "Invite";
		public const string Buy = "Buy";
		public const string Sell = "Sell";
		public const string Get = "Get";
		public const string Drop = "Drop";
	}

	/// <summary>
	/// Represents a collection of Actions.
	/// </summary>
	public class ActionCollection : ActorOwnedDictionaryBase<bool>
	{
		/// <summary>
		/// Initializes a new instance of the ActionCollection class.
		/// </summary>
		/// <param name="owner">Speifies the Actor that owns the collection of Actions.</param>
		public ActionCollection(Actor owner)
			: base(owner, "Action_")
		{
		}

		public override bool TryGetValue(string key, out bool value)
		{
			value = false;
			if (this.Owner.Properties.ContainsKey(this.GetPrefixedName(key)))
			{
				value = this.Owner.Properties.GetValue<bool>(this.GetPrefixedName(key));
				return true;
			}
			return false;
		}
	}
}

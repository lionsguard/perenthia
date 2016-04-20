using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	public static class StringsInternal
	{
		#region Alias
		/// <summary>
		/// Gets the aliased name of the current Actor.
		/// </summary>
		/// <param name="actor">The actor for which to return the aliased name.</param>
		/// <returns>The aliased name of the actor, spaces removed and ID appended.</returns>
		public static string Alias(this IActor actor)
		{
			return actor.Name.Alias(actor.ID);
		}
		#endregion

		#region The
		public static string The(this IActor actor)
		{
			return actor.Name.The(actor.HasProperName, 0);
		}
		public static string The(this IActor actor, int quantity)
		{
			return actor.Name.The(actor.HasProperName, quantity);
		}
		public static string TheUpper(this IActor actor)
		{
			return actor.Name.TheUpper(actor.HasProperName, 0);
		}
		public static string TheUpper(this IActor actor, int quantity)
		{
			return actor.Name.TheUpper(actor.HasProperName, quantity);
		}
		#endregion

		#region A
		public static string A(this IActor actor)
		{
			return actor.Name.A(actor.HasProperName, 0);
		}
		public static string A(this IActor actor, int quantity)
		{
			return actor.Name.A(actor.HasProperName, quantity);
		}
		public static string AUpper(this IActor actor)
		{
			return actor.Name.AUpper(actor.HasProperName, 0);
		}
		public static string AUpper(this IActor actor, int quantity)
		{
			return actor.Name.AUpper(actor.HasProperName, quantity);
		}
		#endregion

		#region Your
		public static string Your(this IActor actor)
		{
			return actor.Name.Your(actor.HasProperName, 0);
		}
		public static string Your(this IActor actor, int quantity)
		{
			return actor.Name.Your(actor.HasProperName, quantity);
		}
		public static string YourUpper(this IActor actor)
		{
			return actor.Name.YourUpper(actor.HasProperName, 0);
		}
		public static string YourUpper(this IActor actor, int quantity)
		{
			return actor.Name.YourUpper(actor.HasProperName, quantity);
		}
		#endregion

		#region None
		public static string None(this IActor actor)
		{
			return actor.Name.None(actor.HasProperName, 0);
		}
		public static string None(this IActor actor, int quantity)
		{
			return actor.Name.None(actor.HasProperName, quantity);
		}
		public static string NoneUpper(this IActor actor)
		{
			return actor.Name.NoneUpper(actor.HasProperName, 0);
		}
		public static string NoneUpper(this IActor actor, int quantity)
		{
			return actor.Name.NoneUpper(actor.HasProperName, quantity);
		}
		#endregion
	}
}

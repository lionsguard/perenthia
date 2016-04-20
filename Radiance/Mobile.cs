using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	/// <summary>
	/// Represents avatars controlled by the virtual world such as NPCs or creatures.
	/// </summary>
	public class Mobile : Avatar
	{
		/// <summary>
		/// Gets or sets the flags controlling the type of the current mobile. The flags control what events and AI is executed.
		/// </summary>
		public MobileTypes MobileType { get; set; }

		/// <summary>
		/// Initializes a new instance of the Mobile class.
		/// </summary>
		public Mobile()
			: base()
		{
			this.MobileType = MobileTypes.None;
		}
	}
}

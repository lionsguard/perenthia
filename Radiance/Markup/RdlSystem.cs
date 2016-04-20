using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	#region RdlSystem
	/// <summary>
	/// Represents a SYS tag in the Radiance Definition Language (RDL).
	/// </summary>
	public abstract class RdlSystem : RdlTag
	{
		private int _messageIndex;

		/// <summary>
		/// Gets or sets the message that contains information about the system tag.
		/// </summary>
		public string Message
		{
			get { return this.GetArg<string>(_messageIndex); }
			set { this.Args[_messageIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the SYS tag.
		/// </summary>
		/// <param name="typeName">The type name of the tag.</param>
		protected RdlSystem(RdlSystemTypeName typeName)
			: this(typeName.ToString(), String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SYS tag.
		/// </summary>
		/// <param name="typeName">The type name of the tag.</param>
		/// <param name="message">The message containing information about the system tag.</param>
		protected RdlSystem(RdlSystemTypeName typeName, string message)
			: this(typeName.ToString(), message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the SYS tag.
		/// </summary>
		/// <param name="typeName">The type name of the tag.</param>
		/// <param name="message">The message containing information about the system tag.</param>
		protected RdlSystem(string typeName, string message)
			: base(RdlTagName.SYS, typeName)
		{
			_messageIndex = this.GetNextIndex();
			this.Args.Insert(_messageIndex, message);
		}
	}
	#endregion
}

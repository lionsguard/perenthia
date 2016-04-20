using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Radiance.Markup
{
	#region RdlMessage
	/// <summary>
	/// Represents an MSG tag in the Radiance Definition Language (RDL).
	/// </summary>
	public abstract class RdlMessage : RdlTag
	{
		private int _textIndex;

		/// <summary>
		/// Gets or sets the text of the message this tag represents.
		/// </summary>
		public string Text
		{
			get { return this.GetArg<string>(_textIndex); }
			set { this.Args[_textIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the MSG tag.
		/// </summary>
		protected RdlMessage()
			: this(String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MSG tag.
		/// </summary>
		protected RdlMessage(string text)
			: base(RdlTagName.MSG)
		{
			_textIndex = this.GetNextIndex();
			this.Args.Insert(_textIndex, text ?? String.Empty);
		}
	}
	#endregion

	#region RdlNewsMessage
	/// <summary>
	/// Represents an MSG|NEWS tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlNewsMessage : RdlMessage
	{
		private int _dateIndex;
		private int _authorIndex;
		private int _titleIndex;

		/// <summary>
		/// Gets the type name of the NEWS tag.
		/// </summary>
		public static readonly string NewsTypeName = "NEWS";

		/// <summary>
		/// Gets or sets the date of the news article this tag represents.
		/// </summary>
		public DateTime Date
		{
			get { return this.GetArg<DateTime>(_dateIndex); }
			set { this.Args[_dateIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the author of the news article this tag represents.
		/// </summary>
		public string Author
		{
			get { return this.GetArg<string>(_authorIndex); }
			set { this.Args[_authorIndex] = value; }
		}

		/// <summary>
		/// Gets or sets the title of the news article this tag represents.
		/// </summary>
		public string Title
		{
			get { return this.GetArg<string>(_titleIndex); }
			set { this.Args[_titleIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the MSG|NEWS tag.
		/// </summary>
		public RdlNewsMessage()
			: this(DateTime.MinValue, String.Empty, String.Empty, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MSG|NEWS tag.
		/// </summary>
		public RdlNewsMessage(DateTime date, string title, string author, string text)
			: base(text)
		{
			this.TypeName = NewsTypeName;
			_dateIndex = this.GetNextIndex();
			_titleIndex = this.GetNextIndex();
			_authorIndex = this.GetNextIndex();
			this.Args.Insert(_dateIndex, date);
			this.Args.Insert(_titleIndex, title);
			this.Args.Insert(_authorIndex, author);
			
		}
	}
	#endregion

	#region RdlSystemMessage
	/// <summary>
	/// Represents an MSG|SYSTEM tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlSystemMessage : RdlMessage
	{
		public enum PriorityType
		{
			None = 0,
			PlaceDescription = 1,
			Admin = 2,
			Positive = 3,
			Negative = 4,
			Neutral = 5,
			Emote = 6,
			PlaceName = 7,
			PlaceExits = 8,
			PlaceActors = 9,
			PlaceAvatars = 10,
			Help = 11,
			Level = 12,
			Cast = 13,
			Melee = 14,
            Award = 15,
		}

		private int _priorityIndex;

		/// <summary>
		/// Gets the type name of the SYSTEM tag.
		/// </summary>
		public static readonly string SystemTypeName = "SYSTEM";

		/// <summary>
		/// Gets or sets the priority of the system message this tag represents.
		/// </summary>
		public int Priority
		{
			get { return this.GetArg<int>(_priorityIndex); }
			set { this.Args[_priorityIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the MSG|SYSTEM tag.
		/// </summary>
		public RdlSystemMessage()
			: this(0, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MSG|NEWS tag.
		/// </summary>
		public RdlSystemMessage(int priority, string text)
			: base(text)
		{
			this.TypeName = SystemTypeName;
			_priorityIndex = this.GetNextIndex();
			this.Args.Insert(_priorityIndex, priority);
			
		}

		/// <summary>
		/// Initializes a new instance of the MSG|NEWS tag.
		/// </summary>
		public RdlSystemMessage(PriorityType priority, string text)
			: this((int)priority, text)
		{
		}
	}
	#endregion

	#region RdlErrorMessage
	/// <summary>
	/// Represents an MSG|ERROR tag in the Radiance Definition Language (RDL).
	/// </summary>
	public partial class RdlErrorMessage : RdlMessage
	{
		/// <summary>
		/// Gets the type name of the ERROR tag.
		/// </summary>
		public static readonly string ErrorTypeName = "ERROR";

		/// <summary>
		/// Initializes a new instance of the MSG|ERROR tag.
		/// </summary>
		public RdlErrorMessage()
			: this(String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MSG|NEWS tag.
		/// </summary>
		public RdlErrorMessage(string text)
			: base(text)
		{
			this.TypeName = ErrorTypeName;			
		}
	}
	#endregion

	#region RdlChatMessage
	/// <summary>
	/// Represents an MSG|CHAT tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlChatMessage : RdlMessage
	{
		private int _fromIndex;

		/// <summary>
		/// Gets the type name of the CHAT tag.
		/// </summary>
		public static readonly string ChatTypeName = "CHAT";

		/// <summary>
		/// Gets or sets the name of sender of the chat message this tag represents.
		/// </summary>
		public string From
		{
			get { return this.GetArg<string>(_fromIndex); }
			set { this.Args[_fromIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the MSG|CHAT tag.
		/// </summary>
		public RdlChatMessage()
			: this(String.Empty, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MSG|NEWS tag.
		/// </summary>
		public RdlChatMessage(string from, string text)
			: base(text)
		{
			this.TypeName = ChatTypeName;
			_fromIndex = this.GetNextIndex();
			this.Args.Insert(_fromIndex, from);
			
		}
	}
	#endregion

	#region RdlTellMessage
	/// <summary>
	/// Represents an MSG|TELL tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlTellMessage : RdlMessage
	{
		private int _fromIndex;

		/// <summary>
		/// Gets the type name of the TELL tag.
		/// </summary>
		public static readonly string TellTypeName = "TELL";

		/// <summary>
		/// Gets or sets the name of sender of the tell message this tag represents.
		/// </summary>
		public string From
		{
			get { return this.GetArg<string>(_fromIndex); }
			set { this.Args[_fromIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the MSG|TELL tag.
		/// </summary>
		public RdlTellMessage()
			: this(String.Empty, String.Empty)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MSG|NEWS tag.
		/// </summary>
		public RdlTellMessage(string from, string text)
			: base(text)
		{
			this.TypeName = TellTypeName;
			_fromIndex = this.GetNextIndex();
			this.Args.Insert(_fromIndex, from);
			
		}
	}
	#endregion
}

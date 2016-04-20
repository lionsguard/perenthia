using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	#region RdlUser
	/// <summary>
	/// Represents a USER tag in the Radiance Definition Language (RDL).
	/// </summary>
	public class RdlUser : RdlTag
	{
		private int _maxCharactersIndex;

		/// <summary>
		/// Gets or sets the max number of characters this user can create.
		/// </summary>
		public int MaxCharacters
		{
			get { return this.GetArg<int>(_maxCharactersIndex); }
			set { this.Args[_maxCharactersIndex] = value; }
		}

		/// <summary>
		/// Initializes a new instance of the USER tag.
		/// </summary>
		public RdlUser()
			: base(RdlTagName.USER)
		{
			this.TypeName = "USER";
			_maxCharactersIndex = this.GetNextIndex();
			this.Args.Insert(_maxCharactersIndex, 0);
		}
	}
	#endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Security
{
	/// <summary>
	/// Represents a user in the radiance virtual world environment.
	/// </summary>
	public class UserDetail
	{
		/// <summary>
		/// Gets or sets the ID of the current user detail instance.
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// Gets or sets the username of the current user.
		/// </summary>
		public string UserName { get; set; }

		/// <summary>
		/// Gets or sets a collection of properties for storing custom user specific data.
		/// </summary>
		public PropertyCollection Properties { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of characters a user can create.
		/// </summary>
		public int MaxCharacters
		{	
			get { return this.Properties.GetValue<int>("MaxCharacters"); }
			set { this.Properties.SetValue("MaxCharacters", value); }
		}

		/// <summary>
		/// Initializes a new instance of the UserDetail class.
		/// </summary>
		public UserDetail()
		{
			this.Properties = new PropertyCollection();
		}
	}
}

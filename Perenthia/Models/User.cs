using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.DataAnnotations;

namespace Perenthia.Models
{
	public class User
	{
		[Required]
		[Display(Name = "User Name", Order = 0, Description = "A unique username for your account.")]
		[StringLength(256)]
		public string UserName { get; set; }

		[Required]
		[Display(Name = "Email Address", Order = 1, Description = "A valid email address used to verify your account. Your email address will remain private and will not be shared for any reason.")]
		[StringLength(256)]
		public string Email { get; set; }

		[Required]
		[Display(Name = "Display Name", Order = 2, Description = "The display name is used when displaying your information instead of your username.")]
		[StringLength(64)]
		public string DisplayName { get; set; }

		[Required]
		[Display(Name = "Password", Order = 3, Description = "Between 6 and 15 characters; try to use numbers, a mix of upper and lower case letters and special characters such as $%@#!")]
		[StringLength(15, MinimumLength=6)]
		public string Password { get; set; }

		[Required]
		[Display(Name = "Confirm Password", Order = 4, Description = "Re-type your password again to ensure it matches the one above.")]
		[StringLength(15, MinimumLength = 6)]
		public string PasswordConfirm { get; set; }

		[Required]
		[Display(Name = "Birth Date", Order = 5, Description = "The day, month and year that you were born. This is used primarily for security purposes.")]
		public DateTime BirthDate { get; set; }

		[Required]
		[Display(Name = "Security Question", Order = 6, Description = "This is a question you will be asked should you forget your password.")]
		[StringLength(256)]
		public string SecurityQuestion { get; set; }

		[Required]
		[Display(Name = "Security Answer", Order = 7, Description = "The answer to the question above.")]
		[StringLength(256)]
		public string SecurityAnswer { get; set; }
            
	}
}

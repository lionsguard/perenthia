using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance
{
	/// <summary>
	/// Provides static utility methods used by the game engine.
	/// </summary>
	internal static class Util
	{
        /// <summary>
		/// Checks the specified args to ensure the number of arguments is at least count and that args are not null. Will also 
		/// add an error message to the specified context.
		/// </summary>
		/// <param name="count"></param>
		/// <param name="args"></param>
		/// <param name="types"></param>
		/// <returns></returns>
		internal static bool CheckArgs(IMessageContext context, int count, object[] args, params Type[] types)
		{
			bool result = true;
			if (args != null && args.Length >= count && types != null && types.Length >= count)
			{
				for (int i = 0; i < count; i++)
				{
					if (args[i] == null) result = false;
					else
					{
						if (args[i].GetType() != types[i]) result = false;
					}
				}
			}
			if (!result)
			{
				AddArgsError(context);
			}
			return result;
		}

		/// <summary>
		/// Adds an incorrect arguments error to the specified context.
		/// </summary>
		/// <param name="context"></param>
		internal static void AddArgsError(IMessageContext context)
		{
			context.Add(new Radiance.Markup.RdlErrorMessage(SR.ArgsInvalid));
		}
	}
}

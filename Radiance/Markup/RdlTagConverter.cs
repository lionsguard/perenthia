using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Radiance.Markup
{
	public static class RdlTagConverter
	{
		/// <summary>
		/// Gets the tag name for the specified type.
		/// </summary>
		/// <typeparam name="T">The RdlTag derived type in which to get the tag name.</typeparam>
		/// <returns>The TagName of the tag.</returns>
		public static string GetTagName<T>() where T : RdlTag
		{
			T tag = Activator.CreateInstance<T>();
			return tag.TagName;
		}

		private static bool TryParseTagName(string tagName, out RdlTagName rdlTagName)
		{
			if (Enum.IsDefined(typeof(RdlTagName), tagName))
			{
				rdlTagName = (RdlTagName)Enum.Parse(typeof(RdlTagName), tagName, true);
				return true;
			}
			rdlTagName = RdlTagName.EMPTY;
			return false;
		}

		public static RdlTag CreateTag(string tagName, string typeName)
		{
			RdlTagName tagNameType = RdlTagName.EMPTY;
			if (TryParseTagName(tagName, out tagNameType))
			{
				switch (tagNameType)
				{
					case RdlTagName.EMPTY:
						return RdlTag.Empty;
					case RdlTagName.OBJ:
						RdlObjectTypeName objTypeName = (RdlObjectTypeName)Enum.Parse(typeof(RdlObjectTypeName), typeName, true);
						switch (objTypeName)
						{
							case RdlObjectTypeName.PROP:
								return new RdlProperty();
							case RdlObjectTypeName.ACTOR:
								return new RdlActor();
							case RdlObjectTypeName.PLAYER:
								return new RdlPlayer();
							case RdlObjectTypeName.PLACE:
								return new RdlPlace();
							case RdlObjectTypeName.RACE:
								return new RdlRace();
							case RdlObjectTypeName.TERRAIN:
								return new RdlTerrain();
							case RdlObjectTypeName.SKILL:
								return new RdlSkill();
						}
						break;
					case RdlTagName.MSG:
						RdlMessageTypeName msgType = (RdlMessageTypeName)Enum.Parse(typeof(RdlMessageTypeName), typeName, true);
						switch (msgType)
						{
							case RdlMessageTypeName.ERROR:
								return new RdlErrorMessage();
							case RdlMessageTypeName.SYSTEM:
								return new RdlSystemMessage();
							case RdlMessageTypeName.NEWS:
								return new RdlNewsMessage();
							case RdlMessageTypeName.CHAT:
								return new RdlChatMessage();
							case RdlMessageTypeName.TELL:
								return new RdlTellMessage();
						}
						break;
					case RdlTagName.CMD:
						return new RdlCommand(typeName);
					case RdlTagName.USER:
						return new RdlUser();
					case RdlTagName.RESP:
						return new RdlCommandResponse(typeName, false, String.Empty);
					case RdlTagName.AUTH:
						return new RdlAuthKey(tagName, typeName);
				}
			}
			return new RdlTag(tagName, typeName);
		}
	}
}

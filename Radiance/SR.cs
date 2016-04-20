using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;

namespace Radiance
{
	public sealed class SR
	{
		#region Properties
		internal static string AttackMiss
		{
			get { return SR.GetString("AttackMiss"); }
		}
		internal static string AttackFailed
		{
			get { return SR.GetString("AttackFailed"); }
		}
		internal static string AttackOutOfRange
		{
			get { return SR.GetString("AttackOutOfRange"); }
		}
		internal static string CastFailed
		{
			get { return SR.GetString("CastFailed"); }
		}
		internal static string CastBackfired
		{
			get { return SR.GetString("CastBackfired"); }
		}
		internal static string CastBackfiredKilledCaster
		{
			get { return SR.GetString("CastBackfiredKilledCaster"); }
		}
		internal static string CastBackfiredUnconsciousCaster
		{
			get { return SR.GetString("CastBackfiredUnconsciousCaster"); }
		}
		internal static string CastChanneling
		{
			get { return SR.GetString("CastChanneling"); }
		}
		internal static string CastNotSelf
		{
			get { return SR.GetString("CastNotSelf"); }
		}
		internal static string MsgTellFormat
		{
			get { return SR.GetString("MsgTellFormat"); }
		}
		internal static string MsgTellFormatEcho
		{
			get { return SR.GetString("MsgTellFormatEcho"); }
		}
		internal static string MsgSayFormat
		{
			get { return SR.GetString("MsgSayFormat"); }
		}
		internal static string MsgSayFormatEcho
		{
			get { return SR.GetString("MsgSayFormatEcho"); }
		}
		internal static string MsgShoutFormat
		{
			get { return SR.GetString("MsgShoutFormat"); }
		}
		internal static string MsgShoutFormatEcho
		{
			get { return SR.GetString("MsgShoutFormatEcho"); }
		}
		internal static string ConfigWorldFileNotFound
		{
			get { return SR.GetString("ConfigWorldFileNotFound"); }
		}
		internal static string ArgsInvalid
		{
			get { return SR.GetString("ArgsInvalid"); }
		}
		internal static string MsgTellNoAvatarDefined
		{
			get { return SR.GetString("MsgTellNoAvatarDefined"); }
		}
		internal static string InvalidCommand
		{
			get { return SR.GetString("InvalidCommand"); }
		}
		internal static string InternalError
		{
			get { return SR.GetString("InternalError"); }
		}
		internal static string ConfigRadianceSectionNotFound
		{
			get { return SR.GetString("ConfigRadianceSectionNotFound"); }
		}
		internal static string ConfigDefaultWorldProviderNotFound
		{
			get { return SR.GetString("ConfigDefaultWorldProviderNotFound"); }
		}
		internal static string ConfigDefaultLogProviderNotFound
		{
			get { return SR.GetString("ConfigDefaultLogProviderNotFound"); }
		}
		internal static string ConfigDefaultCryptoProviderNotFound
		{
			get { return SR.GetString("ConfigDefaultCryptoProviderNotFound"); }
		}
		internal static string LoginRequired
		{
			get { return SR.GetString("LoginRequired"); }
		}
		internal static string CreateCharacterSkillPointsOverLimit
		{
			get { return SR.GetString("CreateCharacterSkillPointsOverLimit"); }
		}
		internal static string CreateCharacterAttributePointsOverLimit
		{
			get { return SR.GetString("CreateCharacterAttributePointsOverLimit"); }
		}
		internal static string NameValidationInvalidLengthShort
		{
			get { return SR.GetString("NameValidationInvalidLengthShort"); }
		}
		internal static string NameValidationInvalidLengthLong
		{
			get { return SR.GetString("NameValidationInvalidLengthLong"); }
		}
		internal static string NameValidationInvalidCharacters
		{
			get { return SR.GetString("NameValidationInvalidCharacters"); }
		}
		internal static string NameValidationInvalidName
		{
			get { return SR.GetString("NameValidationInvalidName"); }
		}
		internal static string CreateCharacterInvalidGender
		{
			get { return SR.GetString("CreateCharacterInvalidGender"); }
		}
		internal static string CreateCharacterInvalidRace
		{
			get { return SR.GetString("CreateCharacterInvalidRace"); }
		}
		internal static string CreateCharacterSuccess
		{
			get { return SR.GetString("CreateCharacterSuccess"); }
		}
		internal static string CreateCharacterInvalidUser
		{
			get { return SR.GetString("CreateCharacterInvalidUser"); }
		}
		internal static string NameValidationNameAlreadyExists
		{
			get { return SR.GetString("NameValidationNameAlreadyExists"); }
		}
		internal static string AttackYouAreUnconscious
		{
			get { return SR.GetString("AttackYouAreUnconscious"); }
		}
		#endregion

		#region Methods
		internal static string AttackYouKilledDefender(params object[] args)
		{
			return SR.GetString("AttackYouKilledDefender", args);
		}
		internal static string AttackYouWereKilledByAttacker(params object[] args)
		{
			return SR.GetString("AttackYouWereKilledByAttacker", args);
		}
		internal static string AttackUnconsciousDefender(params object[] args)
		{
			return SR.GetString("AttackUnconsciousDefender", args);
		}
		internal static string AttackHitDefender(params object[] args)
		{
			return SR.GetString("AttackHitDefender", args);
		}
		internal static string AttackHitByAttacker(params object[] args)
		{
			return SR.GetString("AttackHitByAttacker", args);
		}
		internal static string AttackAttackerFailed(params object[] args)
		{
			return SR.GetString("AttackAttackerFailed", args);
		}
		internal static string CastDamageTarget(params object[] args)
		{
			return SR.GetString("CastDamageTarget", args);
		}
		internal static string CastTargetDamaged(params object[] args)
		{
			return SR.GetString("CastTargetDamaged", args);
		}
		internal static string CastKilledTarget(params object[] args)
		{
			return SR.GetString("CastKilledTarget", args);
		}
		internal static string CastTargetKilled(params object[] args)
		{
			return SR.GetString("CastTargetKilled", args);
		}
		internal static string CastSelfBuff(params object[] args)
		{
			return SR.GetString("CastSelfBuff", args);
		}
		internal static string CastTargetBuff(params object[] args)
		{
			return SR.GetString("CastTargetBuff", args);
		}
		internal static string CastDestroyedObject(params object[] args)
		{
			return SR.GetString("CastDestroyedObject", args);
		}
		internal static string CastTargetUnconscious(params object[] args)
		{
			return SR.GetString("CastTargetUnconscious", args);
		}
		internal static string CastUnconsciousTarget(params object[] args)
		{
			return SR.GetString("CastUnconsciousTarget", args);
		}
		internal static string CastNoAffect(params object[] args)
		{
			return SR.GetString("CastNoAffect", args);
		}
		internal static string CastHealSelf(params object[] args)
		{
			return SR.GetString("CastHealSelf", args);
		}
		internal static string CastHealTarget(params object[] args)
		{
			return SR.GetString("CastHealTarget", args);
		}
		internal static string CastTargetHealed(params object[] args)
		{
			return SR.GetString("CastTargetHealed", args);
		}
		internal static string ConfigWorldNotCreated(params object[] args)
		{
			return SR.GetString("ConfigWorldNotCreated", args);
		}
		internal static string MsgTellNotFound(params object[] args)
		{
			return SR.GetString("MsgTellNotFound", args);
		}
		internal static string PlaceAvatarEnter(params object[] args)
		{
			return SR.GetString("PlaceAvatarEnter", args);
		}
		internal static string PlaceAvatarExit(params object[] args)
		{
			return SR.GetString("PlaceAvatarExit", args);
		}
		internal static string ConfigWorldTypeNotFound(params object[] args)
		{
			return SR.GetString("ConfigWorldTypeNotFound", args);
		}
		internal static string AccessDenied(params object[] args)
		{
			return SR.GetString("AccessDenied", args);
		}
		internal static string CreateCharacterAttributeOutOfRange(params object[] args)
		{
			return SR.GetString("CreateCharacterAttributeOutOfRange", args);
		}
		internal static string CreateCharacterSkillOutOfRange(params object[] args)
		{
			return SR.GetString("CreateCharacterSkillOutOfRange", args);
		}
		internal static string CreateCharacterMaxExceeded(params object[] args)
		{
			return SR.GetString("CreateCharacterMaxExceeded", args);
		}
		internal static string SkillAdvanced(params object[] args)
		{
			return SR.GetString("SkillAdvanced", args);
		}
		internal static string CasterCastFailed(params object[] args)
		{
			return SR.GetString("CasterCastFailed", args);
		}
		internal static string CastSuccess(params object[] args)
		{
			return SR.GetString("CastSuccess", args);
		}
		internal static string AttributeFaded(params object[] args)
		{
			return SR.GetString("AttributeFaded", args);
		}
		internal static string AttributeAffectedIncreased(params object[] args)
		{
			return SR.GetString("AttributeAffectedIncreased", args);
		}
		internal static string AttributeAffectedDecreased(params object[] args)
		{
			return SR.GetString("AttributeAffectedDecreased", args);
		}
		internal static string WorldOffline(params object[] args)
		{
			return SR.GetString("WorldOffline", args);
		}
		#endregion

		private static ResourceManager _resources;

		static SR()
		{
			_resources = new ResourceManager("Radiance.Resources.Resource", typeof(SR).Assembly);
		}

		public static string GetString(string name)
		{
			return _resources.GetString(name);
		}

		public static string GetString(string name, params object[] args)
		{
			string text = GetString(name);
			if (args != null && args.Length > 0)
			{
				return String.Format(text, args);
			}
			return text;
		}
	}
}

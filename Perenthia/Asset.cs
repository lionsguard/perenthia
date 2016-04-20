using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Perenthia
{
	public class Asset
	{
		public const string AVATAR_BLANK = "avatar-blank.png";
		public const string AVATAR_FORMAT = "avatar-{0}-{1}.png";
		public const string AVATAR_NORVIC_MALE = "avatar-norvic-male.png";
		public const string AVATAR_NORVIC_FEMALE = "avatar-norvic-female.png";
		public const string AVATAR_PEREN_MALE = "avatar-peren-male.png";
		public const string AVATAR_PEREN_FEMALE = "avatar-peren-female.png";
		public const string AVATAR_NAJII_MALE = "avatar-najii-male.png";
		public const string AVATAR_NAJII_FEMALE = "avatar-najii-female.png";
		public const string AVATAR_XHIN_MALE = "avatar-xhin-male.png";
		public const string AVATAR_XHIN_FEMALE = "avatar-xhin-female.png";

		public const string PROF_FORMAT = "prof-{0}.png";
		public const string PROF_KNIGHT = "prof-knight.png";
		public const string PROF_MAGE = "prof-mage.png";
		public const string PROF_ROGUE = "prof-rogue.png";
		public const string PROF_RANGER = "prof-ranger.png";

		public static readonly string AssetPath = "/Perenthia;component/Assets/";

		public static Uri GetUri(string path)
		{
			return new Uri(String.Concat(AssetPath, path), UriKind.Relative);
		}

		public static ImageSource GetImageSource(Uri imageUri)
		{
			return new System.Windows.Media.Imaging.BitmapImage(imageUri);
		}

		public static ImageSource GetImageSource(string path)
		{
			Uri uri;
			ImageSource source = null;
			if (path.StartsWith("http"))
			{
				if (Uri.TryCreate(path, UriKind.Absolute, out uri))
				{
					source = Asset.GetImageSource(uri);
				}
				if (source != null) return source;
				return Asset.GetImageSource(Asset.GetUri(path));
			}
			else
			{
				source = ImageManager.GetImageSource(path);
				if (source != null) return source;
				return Asset.GetImageSource(Asset.GetUri(path));
			}
		}

		//public static Uri GetRaceImageUri(RaceType race, Gender gender)
		//{
		//    if (race != RaceType.None && gender != Gender.None)
		//    {
		//        return Asset.GetUri(String.Format(Asset.AVATAR_FORMAT, race, gender));
		//    }
		//    return Asset.GetUri(Asset.AVATAR_BLANK);
		//}

		//public static Uri GetProfessionImageUri(ProfessionType profession)
		//{
		//    if (profession != ProfessionType.None)
		//    {
		//        return Asset.GetUri(String.Format(Asset.PROF_FORMAT, profession));
		//    }
		//    return Asset.GetUri(Asset.AVATAR_BLANK);
		//}
	}
}

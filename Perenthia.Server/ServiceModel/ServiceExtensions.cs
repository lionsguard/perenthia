using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Contract;

namespace Perenthia.ServiceModel
{
	public static class ServiceExtensions
	{
		public static RaceData ToServiceObject(this Race race)
		{
			var obj = new RaceData();
			obj.ID = race.ID;
			obj.Name = race.Name;
			obj.Type = (int)ObjectType.Race;
			obj.Description = race.Description;
			List<NameValuePair> attributes = new List<NameValuePair>();
			foreach (var attr in race.Attributes.Where(a => a.Value != 0))
			{
				attributes.Add(new NameValuePair { Name = attr.Key, Value = attr.Value, Description = AttributeList.Descriptions[attr.Key] });
			}
			obj.Attributes = attributes;
			return obj;
		}

		public static SkillData ToServiceObject(this Skill skill)
		{
			var obj = new SkillData();
			obj.ID = skill.ID;
			obj.Name = skill.Name;
			obj.Type = (int)ObjectType.Actor;
			obj.Description = skill.Description;
			obj.GroupName = skill.GroupName;
			return obj;
		}

		public static AvatarData ToServiceObject(this IAvatar avatar)
		{
			return avatar.ToServiceObject(false);
		}
		public static AvatarData ToServiceObject(this IAvatar avatar, bool includeSkills)
		{
			var obj = new AvatarData();
			obj.ID = avatar.ID;
			obj.Gender = avatar.Gender.ToString();
			obj.Health = avatar.Body;
			obj.HealthMax = avatar.BodyMax;
			obj.IsOnline = Game.Server.World.IsAvatarOnline(avatar);
			obj.Name = avatar.Name;
			obj.Race = avatar.Race;
			obj.Type = (int)avatar.ObjectType;
			obj.Willpower = avatar.Mind;
			obj.WillpowerMax = avatar.MindMax;
			obj.X = avatar.X;
			obj.Y = avatar.Y;
			obj.Z = avatar.Z;
			obj.Level = avatar.Properties.GetValue<int>(PerenthiaAvatar.LevelProperty);

			if (includeSkills)
			{
				List<NameValuePair> skills = new List<NameValuePair>();
				foreach (var skill in avatar.Skills)
				{
					skills.Add(new NameValuePair { Name = skill.Key, Value = (int)skill.Value });
				}
				obj.Skills = skills.ToArray();
			}

			if (avatar is Character)
			{
				Character c = avatar as Character;
				if (c.Household != null)
				{
					obj.HouseholdImageUri = c.Household.HouseholdImageUri;
					obj.HouseholdName = c.Household.HouseholdName;
					obj.RankImageUri = c.Household.RankImageUri;
					obj.RankName = c.Household.RankName;
					obj.RankOrder = c.Household.RankOrder;
				}
				//List<AwardData> awards = new List<AwardData>();
				//foreach (var a in c.Awards)
				//{
				//    awards.Add(new AwardData
				//    {
				//        Name = a.Name,
				//        ImageUri = String.Concat(ConfigurationManager.AppSettings["MediaUri"], a.ImageUri),
				//        Type = a.ObjectType
				//    });
				//}
				//obj.Awards = awards.ToArray();
			}

			MapManager.MapDetail detail = Game.Server.World.Map.GetDetail(avatar.Location);
			if (detail != null)
			{
				obj.Zone = detail.Name;
			}
			return obj;
		}

		public static HouseholdData ToServiceObject(this Household obj)
		{
			var household = new HouseholdData();
			household.Description = obj.Description;
			household.ID = obj.ID;
			household.ImageUri = obj.ImageUri;
			household.Motto = obj.Motto;
			household.Name = obj.Name;
			household.HonorPoints = obj.HonorPoints;
			household.MemberCount = obj.MemberCount;
			household.DateCreated = obj.DateCreated;
			return household;
		}

		public static IEnumerable<AvatarData> ToServiceArray(this IEnumerable<IPlayer> collection)
		{
			return collection.Cast<IAvatar>().ToServiceArray();
		}

		public static IEnumerable<AvatarData> ToServiceArray(this IEnumerable<IAvatar> collection)
		{
			var list = new List<AvatarData>();
			foreach (var item in collection)
			{
				list.Add(item.ToServiceObject());
			}
			return list;
		}

		public static IEnumerable<HouseholdData> ToServiceArray(this IEnumerable<Household> collection)
		{
			var list = new List<HouseholdData>();
			foreach (var item in collection)
			{
				list.Add(item.ToServiceObject());
			}
			return list;
		}
	}
}

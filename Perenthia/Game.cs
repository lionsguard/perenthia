using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;

using Lionsguard;

using Radiance;
using Radiance.Markup;
using Perenthia.Models;
using System.IO;

namespace Perenthia
{
	public static class Game
	{
		public static List<Skill> Skills { get; set; }

		public static List<Terrain> Terrain { get; set; }

		public static Dictionary<string, Race> Races { get; set; }

		public static Dictionary<string, List<Skill>> SkillGroups { get; set; }

		public static List<MapDetail> Maps { get; set; }

		public static Avatar Player { get; set; }
		public static Avatar Target { get; set; }

        public static DateTime PlayerLastSave = DateTime.MinValue;

		public static DragDropManager DragDropManager { get; set; }

		public static string LastTellReceivedFrom { get; set; }

		public static bool ProcessInput { get; set; }

		public static FocusState FocusState { get; set; }

		static Game()
		{
			Skills = new List<Skill>();
			SkillGroups = new Dictionary<string, List<Skill>>(StringComparer.InvariantCultureIgnoreCase);
			Terrain = new List<Terrain>();
			Races = new Dictionary<string, Race>(StringComparer.InvariantCultureIgnoreCase);
			Maps = new List<MapDetail>();
			ProcessInput = true;
			FocusState = FocusState.Main;
		}

		public static void Initialize()
		{
			ProcessSkills(GetRdlTagsFromResource("skills.txt"));
			ProcessSkillGroups(GetRdlTagsFromResource("skillgroups.txt"));
			ProcessTerrain(GetRdlTagsFromResource("terrain.txt"));
			ProcessRaces(GetRdlTagsFromResource("races.txt"));
		}

		#region Process Skills, SkillGroups, Races and Terrain
		private static void ProcessSkills(RdlTagCollection tags)
		{
			List<RdlSkill> skills = tags.GetTags<RdlSkill>(RdlTagName.OBJ.ToString(), RdlObjectTypeName.SKILL.ToString());
			if (skills.Count > 0)
			{
				Game.Skills.Clear();
				foreach (var item in skills)
				{
					Game.Skills.Add(new Skill { Name = item.Name, Description = item.Description, Value = item.Value, GroupName = item.GroupName });
				}
			}
		}
		private static void ProcessSkillGroups(RdlTagCollection tags)
		{
			List<RdlActor> groups = tags.GetTags<RdlActor>(RdlTagName.OBJ.ToString(), RdlObjectTypeName.ACTOR.ToString());
			List<RdlProperty> skills = tags.GetTags<RdlProperty>(RdlTagName.OBJ.ToString(), RdlObjectTypeName.PROP.ToString());
			if (groups.Count > 0)
			{
				Game.SkillGroups.Clear();
				foreach (var group in groups)
				{
					List<Skill> list = new List<Skill>();
					foreach (var skill in skills.Where(s => s.ID == group.ID))
					{
						list.Add(new Skill { Name = skill.Name, Value = Convert.ToInt32(skill.Value) });
					}
					Game.SkillGroups.Add(group.Name, list);
				}
			}
		}
		private static void ProcessRaces(RdlTagCollection tags)
		{
			List<RdlRace> races = tags.GetTags<RdlRace>(RdlTagName.OBJ.ToString(), RdlObjectTypeName.RACE.ToString());
			if (races.Count > 0)
			{
				Game.Races.Clear();
				RdlTagCollection raceTags = new RdlTagCollection();
				foreach (var item in races)
				{
					raceTags.Add(item);
					Race race = new Race
					{
						Name = item.Name,
						Description = item.Description,
					};
					RdlProperty prop = tags.GetProperty(item.ID, "Attr_Strength");
					if (prop != null)
					{
						race.Strength = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Dexterity");
					if (prop != null)
					{
						race.Dexterity = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Stamina");
					if (prop != null)
					{
						race.Stamina = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Beauty");
					if (prop != null)
					{
						race.Beauty = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Intelligence");
					if (prop != null)
					{
						race.Intelligence = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Perception");
					if (prop != null)
					{
						race.Perception = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Endurance");
					if (prop != null)
					{
						race.Endurance = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}
					prop = tags.GetProperty(item.ID, "Attr_Affinity");
					if (prop != null)
					{
						race.Affinity = Convert.ToInt32(prop.Value);
						raceTags.Add(prop);
					}

					Game.Races.Add(item.Name, race);
				}
			}
		}
		private static void ProcessTerrain(RdlTagCollection tags)
		{
			List<RdlTerrain> terrain = tags.GetObjects<RdlTerrain>();
			if (terrain.Count > 0)
			{
				Game.Terrain.Clear();
				foreach (var t in terrain)
				{
					Game.Terrain.Add(new Terrain { ID = t.ID, Name = t.Name, Color = t.Color, ImageUri = t.ImageUrl });
				}
			}
		}
		#endregion

		private static RdlTagCollection GetRdlTagsFromResource(string fileName)
		{
			var info = Application.GetResourceStream(new Uri(String.Concat(Asset.AssetPath, "Data/", fileName), UriKind.Relative));
			if (info == null || info.Stream == null)
				return new RdlTagCollection();

			using (var sr = new StreamReader(info.Stream))
			{
				return RdlTagCollection.FromString(sr.ReadToEnd());
			}
		}

		public static void LoadMapDetails(IEnumerable<RdlTag> mapTags)
		{
			Maps.Clear();
			foreach (var detail in mapTags)
			{
				Maps.Add(new MapDetail(detail));
			}
		}

		public static void EnsureSkillDetails(IEnumerable<Skill> skills)
		{
			foreach (var skill in skills)
			{
				if (String.IsNullOrEmpty(skill.GroupName) || String.IsNullOrEmpty(skill.Description))
				{
					var s = Game.Skills.Where(t => t.Name == skill.Name.Replace("Skill_", String.Empty)).FirstOrDefault();
					if (s != null)
					{
						skill.GroupName = s.GroupName;
						skill.Description = s.Description;
					}
				}
			}
		}
	}
}

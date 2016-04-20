using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Reflection;

using Radiance;
using Perenthia;

namespace Perenthia
{
    public static class DataExtensions
    {
		public static DataTable ToDataTable(this IEnumerable<IPlayer> avatars)
		{
			return avatars.Cast<IAvatar>().ToDataTable();
		}

		public static DataTable ToDataTable(this IEnumerable<IAvatar> avatars)
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("ID", typeof(int));
			dt.Columns.Add("Name", typeof(String));
			dt.Columns.Add("Race", typeof(String));
			dt.Columns.Add("Gender", typeof(Gender));
			dt.Columns.Add("Health", typeof(int));
			dt.Columns.Add("HealthMax", typeof(int));
			dt.Columns.Add("Willpower", typeof(int));
			dt.Columns.Add("WillpowerMax", typeof(int));
			dt.Columns.Add("X", typeof(int));
			dt.Columns.Add("Y", typeof(int));
			dt.Columns.Add("Z", typeof(int));
			dt.Columns.Add("Type", typeof(ObjectType));
			dt.Columns.Add("Level", typeof(int));
			dt.Columns.Add("IsOnline", typeof(bool));
			dt.Columns.Add("HouseholdName", typeof(String));
			dt.Columns.Add("HouseholdImageUri", typeof(String));
			dt.Columns.Add("RankName", typeof(String));
			dt.Columns.Add("RankImageUri", typeof(String));
			dt.Columns.Add("Zone", typeof(String));

			foreach (var avatar in avatars)
			{
				DataRow dr = dt.NewRow();
				dr["ID"] = avatar.ID;
				dr["Name"] = avatar.Name;
				dr["Race"] = avatar.Race;
				dr["Gender"] = avatar.Gender;
				dr["Health"] = avatar.Body;
				dr["HealthMax"] = avatar.BodyMax;
				dr["Willpower"] = avatar.Mind;
				dr["WillpowerMax"] = avatar.MindMax;
				dr["X"] = avatar.X;
				dr["Y"] = avatar.Y;
				dr["Z"] = avatar.Z;
				dr["Type"] = avatar.ObjectType;
				dr["Level"] = avatar.Properties.GetValue<int>(PerenthiaAvatar.LevelProperty);
				dr["IsOnline"] = Game.Server.World.IsAvatarOnline(avatar);

				if (avatar is IPlayer)
				{
					var player = avatar as IPlayer;
					dr["HouseholdName"] = player.Household.HouseholdName;
					dr["HouseholdImageUri"] = player.Household.HouseholdImageUri;
					dr["RankName"] = player.Household.RankName;
					dr["RankImageUri"] = player.Household.RankImageUri;
				}

				MapManager.MapDetail detail = Game.Server.World.Map.GetDetail(avatar.Location);
				if (detail != null)
				{
					dr["Zone"] = detail.Name;
				}
				dt.Rows.Add(dr);
			}
			return dt;
		}

        public static DataTable ToDataTable<T>(this IEnumerable<T> collection)
        {
            DataTable dt = new DataTable();
            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties();
            foreach (var p in properties)
            {
                dt.Columns.Add(p.Name, p.PropertyType);
            }

            foreach (var item in collection)
            {
                DataRow dr = dt.NewRow();
                foreach (var p in properties)
                {
                    dr[p.Name] = p.GetValue(item, null);
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}

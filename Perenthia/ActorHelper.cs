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

using Radiance;
using Radiance.Markup;
using Perenthia.Models;

namespace Perenthia
{
	public static class ActorHelper
	{
		public static bool HasFlag(RdlActor actor, string flagName)
		{
			return HasFlagInternal(actor.Properties.GetValue<string>("Flags"), flagName);
		}

		public static bool HasFlag(Avatar avatar, string flagName)
		{
			return HasFlagInternal(avatar.Properties.GetValue<string>("Flags"), flagName);
		}

		private static bool HasFlagInternal(string flags, string flagName)
		{
			if (!String.IsNullOrEmpty(flags) && !String.IsNullOrEmpty(flagName))
			{
				ActorFlags actorFlags = (ActorFlags)Enum.Parse(typeof(ActorFlags), flags, true);
				ActorFlags checkFlag = (ActorFlags)Enum.Parse(typeof(ActorFlags), flagName, true);
				return ((actorFlags & checkFlag) == checkFlag);
			}
			return false;
		}

		public static bool IsUsable(this RdlActor actor)
		{
            return actor.Properties.GetValue<bool>("IsUsable");
		}

        public static bool IsStackable(this RdlActor actor)
        {
            return actor.Properties.GetValue<bool>("IsStackable");
        }

        public static bool IsEquipped(this RdlActor actor)
        {
            return actor.Properties.GetValue<bool>("IsEquipped");
        }

        public static int Quantity(this RdlActor actor)
        {
            return actor.Properties.GetValue<int>("Quantity");
        }

        public static string ImageUri(this RdlActor actor)
        {
            return actor.Properties.GetValue<string>("ImageUri");
        }

        public static EquipLocation EquipLocation(this RdlActor actor)
        {
            EquipLocation equipLoc = Radiance.EquipLocation.None;
            string loc = actor.Properties.GetValue<string>("EquipLocation");
            if (!String.IsNullOrEmpty(loc)) equipLoc = (EquipLocation)Enum.Parse(typeof(EquipLocation), loc, true);
            return equipLoc;
        }

        public static ObjectType ObjectType(this RdlActor actor)
        {
            return actor.Properties.GetValue<ObjectType>("ObjectType");
        }
	}
}

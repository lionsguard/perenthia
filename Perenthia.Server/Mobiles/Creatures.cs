using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

// Should be data drive rather than hard coded.
namespace Perenthia.Mobiles
{
	public static class Creatures
	{		
		#region Rat
		public static Creature Rat
		{
			get { return new Creature("Rat", "", 2, 3, 4, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region GiantRat
		public static Creature GiantRat
		{
			get { return new Creature("Giant Rat", "", 3, 3, 5, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region CreeperWorker
		public static Creature CreeperWorker
		{
			get { return new Creature("Creeper Worker", "", 1, 3, 4, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region CreeperHunter
		public static Creature CreeperHunter
		{
			get { return new Creature("Creeper Hunter", "", 3, 3, 4, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region CreeperWarrior
		public static Creature CreeperWarrior
		{
			get { return new Creature("Creeper Warrior", "", 5, 3, 5, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region CreeperMage
		public static Creature CreeperMage
		{
			get { return new Creature("Creeper Mage", "", 7, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region CreeperDrone
		public static Creature CreeperDrone
		{
			get { return new Creature("Creeper Drone", "", 8, 3, 6, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region CreeperQueen
		public static Creature CreeperQueen
		{
			get { return new Creature("Creeper Queen", "", 9, 3, 6, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Skeleton
		public static Creature Skeleton
		{
			get { return new Creature("Skeleton", "", 2, 3, 4, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region SkeletonGuard
		public static Creature SkeletonGuard
		{
			get { return new Creature("Skeleton Guard", "", 4, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region SkeletonWarrior
		public static Creature SkeletonWarrior
		{
			get { return new Creature("Skeleton Warrior", "", 6, 3, 6, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region SkeletonLord
		public static Creature SkeletonLord
		{
			get { return new Creature("Skeleton Lord", "", 8, 3, 6, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region Kobold
		public static Creature Kobold
		{
			get { return new Creature("Kobold", "", 3, 3, 4, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region KoboldScout
		public static Creature KoboldScout
		{
			get { return new Creature("Kobold Scout", "", 5, 3, 5, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region KoboldSentry
		public static Creature KoboldSentry
		{
			get { return new Creature("Kobold Sentry", "", 7, 3, 6, MobileTypes.Sentinal, new ThiefSkillGroup()); }
		}
		#endregion

		#region KoboldSoldier
		public static Creature KoboldSoldier
		{
			get { return new Creature("Kobold Soldier", "", 10, 3, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region KoboldCaster
		public static Creature KoboldCaster
		{
			get { return new Creature("Kobold Caster", "", 11, 3, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region KoboldCommander
		public static Creature KoboldCommander
		{
			get { return new Creature("Kobold Commander", "", 12, 5, 7, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region KoboldLord
		public static Creature KoboldLord
		{
			get { return new Creature("Kobold Lord", "", 14, 5, 7, MobileTypes.Guard, new ThiefSkillGroup()); }
		}
		#endregion

		#region KoboldKing
		public static Creature KoboldKing
		{
			get { return new Creature("Kobold King", "", 18, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Spiderling
		public static Creature Spiderling
		{
			get { return new Creature("Spiderling", "", 2, 3, 4, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region Spider
		public static Creature Spider
		{
			get { return new Creature("Spider", "", 4, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region LargeSpider
		public static Creature LargeSpider
		{
			get { return new Creature("Large Spider", "", 6, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region GiantSpider
		public static Creature GiantSpider
		{
			get { return new Creature("Giant Spider", "", 8, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region BlackWidow
		public static Creature BlackWidow
		{
			get { return new Creature("Black Widow", "", 16, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region Leech
		public static Creature Leech
		{
			get { return new Creature("Leech", "", 1, 3, 3, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region DeathLeech
		public static Creature DeathLeech
		{
			get { return new Creature("Death Leech", "", 3, 3, 4, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region GiantLeech
		public static Creature GiantLeech
		{
			get { return new Creature("Giant Leech", "", 4, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region PoisonousLeech
		public static Creature PoisonousLeech
		{
			get { return new Creature("Poisonous Leech", "", 6, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region Wight
		public static Creature Wight
		{
			get { return new Creature("Wight", "", 4, 3, 4, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region WightWarriror
		public static Creature WightWarriror
		{
			get { return new Creature("Wight Warriror", "", 8, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region WightSummoner
		public static Creature WightSummoner
		{
			get { return new Creature("Wight Summoner", "", 12, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region WightScreamer
		public static Creature WightScreamer
		{
			get { return new Creature("Wight Screamer", "", 16, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region Wraith
		public static Creature Wraith
		{
			get { return new Creature("Wraith", "", 3, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region WraithKnight
		public static Creature WraithKnight
		{
			get { return new Creature("Wraith Knight", "", 7, 5, 7, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region WraithCaller
		public static Creature WraithCaller
		{
			get { return new Creature("Wraith Caller", "", 9, 5, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region WraithDiabolic
		public static Creature WraithDiabolic
		{
			get { return new Creature("Wraith Diabolic", "", 13, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Ghoul
		public static Creature Ghoul
		{
			get { return new Creature("Ghoul", "", 8, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region GhoulWanderer
		public static Creature GhoulWanderer
		{
			get { return new Creature("Ghoul Wanderer", "", 11, 3, 5, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region GhoulFighter
		public static Creature GhoulFighter
		{
			get { return new Creature("Ghoul Fighter", "", 13, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region GhoulSummoner
		public static Creature GhoulSummoner
		{
			get { return new Creature("Ghoul Summoner", "", 15, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GhoulReacher
		public static Creature GhoulReacher
		{
			get { return new Creature("Ghoul Reacher", "", 19, 4, 6, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Goblin
		public static Creature Goblin
		{
			get { return new Creature("Goblin", "", 9, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region GoblinScout
		public static Creature GoblinScout
		{
			get { return new Creature("Goblin Scout", "", 10, 3, 5, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region GoblinWarriror
		public static Creature GoblinWarriror
		{
			get { return new Creature("Goblin Warriror", "", 11, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region GoblinCaster
		public static Creature GoblinCaster
		{
			get { return new Creature("Goblin Caster", "", 12, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GoblinWarlord
		public static Creature GoblinWarlord
		{
			get { return new Creature("Goblin Warlord", "", 15, 4, 6, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region GoblinLord
		public static Creature GoblinLord
		{
			get { return new Creature("Goblin Lord", "", 17, 5, 7, MobileTypes.Guard, new ThiefSkillGroup()); }
		}
		#endregion

		#region GoblinKing
		public static Creature GoblinKing
		{
			get { return new Creature("Goblin King", "", 19, 5, 7, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region GoblinQueen
		public static Creature GoblinQueen
		{
			get { return new Creature("Goblin Queen", "", 19, 5, 7, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Reaver
		public static Creature Reaver
		{
			get { return new Creature("Reaver", "", 13, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region ReaverScout
		public static Creature ReaverScout
		{
			get { return new Creature("Reaver Scout", "", 15, 3, 5, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region ReaverWorker
		public static Creature ReaverWorker
		{
			get { return new Creature("Reaver Worker", "", 16, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region ReaverMage
		public static Creature ReaverMage
		{
			get { return new Creature("Reaver Mage", "", 17, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region ReaverWarrior
		public static Creature ReaverWarrior
		{
			get { return new Creature("Reaver Warrior", "", 18, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region ReaverCaptain
		public static Creature ReaverCaptain
		{
			get { return new Creature("Reaver Captain", "", 21, 4, 6, MobileTypes.Guard, new ThiefSkillGroup()); }
		}
		#endregion

		#region ReaverCommander
		public static Creature ReaverCommander
		{
			get { return new Creature("Reaver Commander", "", 23, 4, 6, MobileTypes.Guard, new ThiefSkillGroup()); }
		}
		#endregion

		#region ReaverDrone
		public static Creature ReaverDrone
		{
			get { return new Creature("Reaver Drone", "", 25, 4, 7, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region ReaverQueen
		public static Creature ReaverQueen
		{
			get { return new Creature("Reaver Queen", "", 29, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Zombie
		public static Creature Zombie
		{
			get { return new Creature("Zombie", "", 17, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region ZombieWanderer
		public static Creature ZombieWanderer
		{
			get { return new Creature("Zombie Wanderer", "", 19, 3, 5, MobileTypes.Roamer, new ThiefSkillGroup()); }
		}
		#endregion

		#region ZombieCaster
		public static Creature ZombieCaster
		{
			get { return new Creature("Zombie Caster", "", 21, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region ZombieWarrior
		public static Creature ZombieWarrior
		{
			get { return new Creature("Zombie Warrior", "", 22, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region ZombieCrusher
		public static Creature ZombieCrusher
		{
			get { return new Creature("Zombie Crusher", "", 23, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region Phantom
		public static Creature Phantom
		{
			get { return new Creature("Phantom", "", 20, 3, 5, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region PhantomMage
		public static Creature PhantomMage
		{
			get { return new Creature("Phantom Mage", "", 22, 3, 5, MobileTypes.Roamer, new CasterSkillGroup()); }
		}
		#endregion

		#region PhantomKiller
		public static Creature PhantomKiller
		{
			get { return new Creature("Phantom Killer", "", 23, 3, 5, MobileTypes.Roamer, new ThiefSkillGroup()); }
		}
		#endregion

		#region PhantomShifter
		public static Creature PhantomShifter
		{
			get { return new Creature("Phantom Shifter", "", 25, 4, 6, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region PhantomKnight
		public static Creature PhantomKnight
		{
			get { return new Creature("Phantom Knight", "", 27, 4, 6, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region PhantomStalker
		public static Creature PhantomStalker
		{
			get { return new Creature("Phantom Stalker", "", 28, 4, 6, MobileTypes.Roamer, new ThiefSkillGroup()); }
		}
		#endregion

		#region PhantomScreamer
		public static Creature PhantomScreamer
		{
			get { return new Creature("Phantom Screamer", "", 31, 5, 7, MobileTypes.Roamer, new CasterSkillGroup()); }
		}
		#endregion

		#region LesserFiend
		public static Creature LesserFiend
		{
			get { return new Creature("Lesser Fiend", "", 26, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region Fiend
		public static Creature Fiend
		{
			get { return new Creature("Fiend", "", 28, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region FiendSummoner
		public static Creature FiendSummoner
		{
			get { return new Creature("Fiend Summoner", "", 29, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region FiendMaster
		public static Creature FiendMaster
		{
			get { return new Creature("Fiend Master", "", 31, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region FiendOverlord
		public static Creature FiendOverlord
		{
			get { return new Creature("Fiend Overlord", "", 33, 5, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region FiendDrainer
		public static Creature FiendDrainer
		{
			get { return new Creature("Fiend Drainer", "", 36, 5, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region FiendPlaneShifter
		public static Creature FiendPlaneShifter
		{
			get { return new Creature("Fiend Plane Shifter", "", 38, 6, 8, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region Hobgoblin
		public static Creature Hobgoblin
		{
			get { return new Creature("Hobgoblin", "", 30, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region HobgoblinScout
		public static Creature HobgoblinScout
		{
			get { return new Creature("Hobgoblin Scout", "", 31, 3, 5, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region HobgoblinSentry
		public static Creature HobgoblinSentry
		{
			get { return new Creature("Hobgoblin Sentry", "", 32, 3, 5, MobileTypes.Sentinal, new ThiefSkillGroup()); }
		}
		#endregion

		#region HobgoblinMage
		public static Creature HobgoblinMage
		{
			get { return new Creature("Hobgoblin Mage", "", 34, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region HobgoblinLord
		public static Creature HobgoblinLord
		{
			get { return new Creature("Hobgoblin Lord", "", 35, 5, 7, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region HobgoblinGodling
		public static Creature HobgoblinGodling
		{
			get { return new Creature("Hobgoblin Godling", "", 37, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region ShadowWolf
		public static Creature ShadowWolf
		{
			get { return new Creature("Shadow Wolf", "", 34, 4, 6, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region ShadowWolfStalker
		public static Creature ShadowWolfStalker
		{
			get { return new Creature("Shadow Wolf Stalker", "", 37, 5, 7, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region ShadowWolfPackLeader
		public static Creature ShadowWolfPackLeader
		{
			get { return new Creature("Shadow Wolf Pack Leader", "", 39, 6, 8, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region Werewolf
		public static Creature Werewolf
		{
			get { return new Creature("Werewolf", "", 35, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region WerewolfScout
		public static Creature WerewolfScout
		{
			get { return new Creature("Werewolf Scout", "", 36, 4, 6, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region WerewolfWarrior
		public static Creature WerewolfWarrior
		{
			get { return new Creature("Werewolf Warrior", "", 37, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region WerewolfSentry
		public static Creature WerewolfSentry
		{
			get { return new Creature("Werewolf Sentry", "", 38, 5, 7, MobileTypes.Sentinal, new ThiefSkillGroup()); }
		}
		#endregion

		#region WerewolfHowler
		public static Creature WerewolfHowler
		{
			get { return new Creature("Werewolf Howler", "", 40, 5, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region WerewolfPackLeader
		public static Creature WerewolfPackLeader
		{
			get { return new Creature("Werewolf Pack Leader", "", 43, 6, 8, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region Troll
		public static Creature Troll
		{
			get { return new Creature("Troll", "", 40, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region TrollScout
		public static Creature TrollScout
		{
			get { return new Creature("Troll Scout", "", 41, 4, 6, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region TrollWarrior
		public static Creature TrollWarrior
		{
			get { return new Creature("Troll Warrior", "", 43, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region TrollShaman
		public static Creature TrollShaman
		{
			get { return new Creature("Troll Shaman", "", 44, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region TrollCaptain
		public static Creature TrollCaptain
		{
			get { return new Creature("Troll Captain", "", 45, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region TrollBeserker
		public static Creature TrollBeserker
		{
			get { return new Creature("Troll Beserker", "", 46, 5, 7, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region TrollElite
		public static Creature TrollElite
		{
			get { return new Creature("Troll Elite", "", 47, 5, 7, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region TrollQueen
		public static Creature TrollQueen
		{
			get { return new Creature("Troll Queen", "", 49, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region TrollKing
		public static Creature TrollKing
		{
			get { return new Creature("Troll King", "", 50, 6, 8, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region TrollGod
		public static Creature TrollGod
		{
			get { return new Creature("Troll God", "", 55, 8, 9, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Ogre
		public static Creature Ogre
		{
			get { return new Creature("Ogre", "", 43, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region OgreCrusher
		public static Creature OgreCrusher
		{
			get { return new Creature("Ogre Crusher", "", 45, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region OgreCaster
		public static Creature OgreCaster
		{
			get { return new Creature("Ogre Caster", "", 46, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region OgreWarrior
		public static Creature OgreWarrior
		{
			get { return new Creature("Ogre Warrior", "", 48, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region OgreWarlord
		public static Creature OgreWarlord
		{
			get { return new Creature("Ogre Warlord", "", 50, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region OgreBodyguard
		public static Creature OgreBodyguard
		{
			get { return new Creature("Ogre Bodyguard", "", 51, 5, 7, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region OgreQueen
		public static Creature OgreQueen
		{
			get { return new Creature("Ogre Queen", "", 55, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region OgreGoddess
		public static Creature OgreGoddess
		{
			get { return new Creature("Ogre Goddess", "", 60, 8, 9, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region LesserBasilisk
		public static Creature LesserBasilisk
		{
			get { return new Creature("Lesser Basilisk", "", 23, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region Basilisk
		public static Creature Basilisk
		{
			get { return new Creature("Basilisk", "", 24, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GreaterBasilisk
		public static Creature GreaterBasilisk
		{
			get { return new Creature("Greater Basilisk", "", 25, 3, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region BasiliskSummoner
		public static Creature BasiliskSummoner
		{
			get { return new Creature("Basilisk Summoner", "", 26, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region BasiliskShifter
		public static Creature BasiliskShifter
		{
			get { return new Creature("Basilisk Shifter", "", 28, 5, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region BasiliskGodling
		public static Creature BasiliskGodling
		{
			get { return new Creature("Basilisk Godling", "", 31, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Siren
		public static Creature Siren
		{
			get { return new Creature("Siren", "", 49, 3, 5, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region SirenNoble
		public static Creature SirenNoble
		{
			get { return new Creature("Siren Noble", "", 51, 3, 5, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region SirenProtector
		public static Creature SirenProtector
		{
			get { return new Creature("Siren Protector", "", 52, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region SirenCaller
		public static Creature SirenCaller
		{
			get { return new Creature("Siren Caller", "", 54, 4, 6, MobileTypes.Roamer, new CasterSkillGroup()); }
		}
		#endregion

		#region SirenPriestess
		public static Creature SirenPriestess
		{
			get { return new Creature("Siren Priestess", "", 56, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region SirenBodyguard
		public static Creature SirenBodyguard
		{
			get { return new Creature("Siren Bodyguard", "", 57, 4, 6, MobileTypes.Guard, new ThiefSkillGroup()); }
		}
		#endregion

		#region SirenPrincess
		public static Creature SirenPrincess
		{
			get { return new Creature("Siren Princess", "", 59, 5, 7, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region SirenQueen
		public static Creature SirenQueen
		{
			get { return new Creature("Siren Queen", "", 63, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Harpie
		public static Creature Harpie
		{
			get { return new Creature("Harpie", "", 47, 3, 5, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region HarpieScout
		public static Creature HarpieScout
		{
			get { return new Creature("Harpie Scout", "", 49, 3, 5, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region HarpieGuard
		public static Creature HarpieGuard
		{
			get { return new Creature("Harpie Guard", "", 50, 4, 6, MobileTypes.Sentinal, new ThiefSkillGroup()); }
		}
		#endregion

		#region HarpieExile
		public static Creature HarpieExile
		{
			get { return new Creature("Harpie Exile", "", 49, 4, 6, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region HarpieElite
		public static Creature HarpieElite
		{
			get { return new Creature("Harpie Elite", "", 52, 5, 7, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region HarpieQueen
		public static Creature HarpieQueen
		{
			get { return new Creature("Harpie Queen", "", 55, 6, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Cyclops
		public static Creature Cyclops
		{
			get { return new Creature("Cyclops", "", 53, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region CyclopsWarriror
		public static Creature CyclopsWarriror
		{
			get { return new Creature("Cyclops Warriror", "", 55, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region CyclopsSentry
		public static Creature CyclopsSentry
		{
			get { return new Creature("Cyclops Sentry", "", 56, 3, 5, MobileTypes.Sentinal, new ThiefSkillGroup()); }
		}
		#endregion

		#region CyclopsLord
		public static Creature CyclopsLord
		{
			get { return new Creature("Cyclops Lord", "", 57, 5, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region CyclopsWarlord
		public static Creature CyclopsWarlord
		{
			get { return new Creature("Cyclops Warlord", "", 59, 6, 7, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region CyclopsQueen
		public static Creature CyclopsQueen
		{
			get { return new Creature("Cyclops Queen", "", 60, 7, 8, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region CyclopsKing
		public static Creature CyclopsKing
		{
			get { return new Creature("Cyclops King", "", 60, 8, 9, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region Griffin
		public static Creature Griffin
		{
			get { return new Creature("Griffin", "", 57, 3, 5, MobileTypes.Roamer, new FighterSkillGroup()); }
		}
		#endregion

		#region GriffinWorker
		public static Creature GriffinWorker
		{
			get { return new Creature("Griffin Worker", "", 59, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region GriffinDrone
		public static Creature GriffinDrone
		{
			get { return new Creature("Griffin Drone", "", 63, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region GriffinQueen
		public static Creature GriffinQueen
		{
			get { return new Creature("Griffin Queen", "", 65, 5, 7, MobileTypes.Guard, new FighterSkillGroup()); }
		}
		#endregion

		#region Minotaur
		public static Creature Minotaur
		{
			get { return new Creature("Minotaur", "", 60, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region MinotaurSentry
		public static Creature MinotaurSentry
		{
			get { return new Creature("Minotaur Sentry", "", 62, 3, 5, MobileTypes.Sentinal, new ThiefSkillGroup()); }
		}
		#endregion

		#region MinotuarWarrior
		public static Creature MinotuarWarrior
		{
			get { return new Creature("Minotuar Warrior", "", 64, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region MinotaurElite
		public static Creature MinotaurElite
		{
			get { return new Creature("Minotaur Elite", "", 67, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region Dragonkin
		public static Creature Dragonkin
		{
			get { return new Creature("Dragonkin", "", 58, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region DragonkinScout
		public static Creature DragonkinScout
		{
			get { return new Creature("Dragonkin Scout", "", 59, 4, 6, MobileTypes.Roamer, new ExplorerSkillGroup()); }
		}
		#endregion

		#region DragonkinWarrior
		public static Creature DragonkinWarrior
		{
			get { return new Creature("Dragonkin Warrior", "", 61, 5, 7, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region DragonkinSentry
		public static Creature DragonkinSentry
		{
			get { return new Creature("Dragonkin Sentry", "", 63, 5, 7, MobileTypes.Sentinal, new ThiefSkillGroup()); }
		}
		#endregion

		#region WyrmHatchling
		public static Creature WyrmHatchling
		{
			get { return new Creature("Wyrm Hatchling", "", 57, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region WyrmAdolescent
		public static Creature WyrmAdolescent
		{
			get { return new Creature("Wyrm Adolescent", "", 59, 4, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region WyrmAdult
		public static Creature WyrmAdult
		{
			get { return new Creature("Wyrm Adult", "", 61, 5, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region WyrmElder
		public static Creature WyrmElder
		{
			get { return new Creature("Wyrm Elder", "", 63, 5, 7, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region DraconianScout
		public static Creature DraconianScout
		{
			get { return new Creature("Draconian Scout", "", 64, 3, 5, MobileTypes.Sentinal, new ExplorerSkillGroup()); }
		}
		#endregion

		#region DraconianWarriror
		public static Creature DraconianWarriror
		{
			get { return new Creature("Draconian Warriror", "", 65, 3, 5, MobileTypes.Sentinal, new FighterSkillGroup()); }
		}
		#endregion

		#region DraconianSentry
		public static Creature DraconianSentry
		{
			get { return new Creature("Draconian Sentry", "", 67, 3, 5, MobileTypes.Sentinal, new ThiefSkillGroup()); }
		}
		#endregion

		#region GreenDragonHatchling
		public static Creature GreenDragonHatchling
		{
			get { return new Creature("Green Dragon Hatchling", "", 100, 4, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GreenDragonAdolescent
		public static Creature GreenDragonAdolescent
		{
			get { return new Creature("Green Dragon Adolescent", "", 110, 5, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GreenDragonAdult
		public static Creature GreenDragonAdult
		{
			get { return new Creature("Green Dragon Adult", "", 120, 6, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GreenDragonElder
		public static Creature GreenDragonElder
		{
			get { return new Creature("Green Dragon Elder", "", 140, 7, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region GoldenDragonHatchling
		public static Creature GoldenDragonHatchling
		{
			get { return new Creature("Golden Dragon Hatchling", "", 150, 4, 5, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GoldenDragonAdolescent
		public static Creature GoldenDragonAdolescent
		{
			get { return new Creature("Golden Dragon Adolescent", "", 160, 5, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GoldenDragonAdult
		public static Creature GoldenDragonAdult
		{
			get { return new Creature("Golden Dragon Adult", "", 180, 6, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region GoldenDragonElder
		public static Creature GoldenDragonElder
		{
			get { return new Creature("Golden Dragon Elder", "", 210, 7, 8, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region RedDragonHatchling
		public static Creature RedDragonHatchling
		{
			get { return new Creature("Red Dragon Hatchling", "", 230, 6, 6, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region RedDragonAdolescent
		public static Creature RedDragonAdolescent
		{
			get { return new Creature("Red Dragon Adolescent", "", 250, 7, 7, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region RedDragonAdult
		public static Creature RedDragonAdult
		{
			get { return new Creature("Red Dragon Adult", "", 270, 8, 8, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region RedDragonElder
		public static Creature RedDragonElder
		{
			get { return new Creature("Red Dragon Elder", "", 300, 9, 9, MobileTypes.Sentinal, new CasterSkillGroup()); }
		}
		#endregion

		#region Demon
		public static Creature Demon
		{
			get { return new Creature("Demon", "", 250, 10, 10, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region Godling
		public static Creature Godling
		{
			get { return new Creature("Godling", "", 275, 12, 12, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region God
		public static Creature God
		{
			get { return new Creature("God", "", 300, 15, 15, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

		#region LordoftheUnderworld
		public static Creature LordoftheUnderworld
		{
			get { return new Creature("Lord of the Underworld", "", 300, 20, 20, MobileTypes.Guard, new CasterSkillGroup()); }
		}
		#endregion

	}
}
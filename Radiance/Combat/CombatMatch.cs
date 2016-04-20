using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Radiance.Markup;

namespace Radiance.Combat
{
	public class CombatMatch
	{
		private const double RoundClientResponseWaitInterval = 25;

		private IAvatar[] _attackers;
		private IAvatar[] _defenders;
		private TimeSpan _roundElapsedTime;
		private bool _isPerformingRound;
		private bool _isMatchComplete;
		private bool _isStarted;

		public Guid ID { get; private set; }
	
		public event CombatMatchCompletedEventHandler Completed = delegate { };

		public CombatMatch(IAvatar[] attackers, IAvatar[] defenders)
		{
			this.ID = Guid.NewGuid();
			_attackers = attackers;
			_defenders = defenders;
		}

		public void Update()
		{
			if (!_isStarted)
				return;

			if (_isMatchComplete)
				return;

			if (_isPerformingRound)
				return;

			// Check to see if either all contestants have responded with a combat action or the 
			// round wait time interval has elapsed.
			_roundElapsedTime += GameTime.ElapsedTime;

			if ((_roundElapsedTime.TotalMilliseconds >= RoundClientResponseWaitInterval)
				|| (CheckActionResponse(_attackers) && CheckActionResponse(_defenders)))
			{
				PerformRound();
			}
		}

		public void Start()
		{
			// Inform all contestants and wait for action responses.
			RequestActionResponse(_attackers);
			RequestActionResponse(_defenders);

			_roundElapsedTime = TimeSpan.Zero;

			_isStarted = true;
		}

		private void PerformRound()
		{
			_isPerformingRound = true;

			// Combine contestants into one array.
			var contestants = new IAvatar[_attackers.Length + _defenders.Length];
			Array.Copy(_attackers, 0, contestants, 0, _attackers.Length);
			Array.Copy(_defenders, 0, contestants, _attackers.Length, _defenders.Length);

			// Roll for initiative.
			CombatManager.DetermineInitiative(InitiativeType.Individual, contestants);

			// Sort the contestants based on initiative.
			Array.Sort<IAvatar>(contestants, new Comparison<IAvatar>((m1, m2) =>
				{
					return (m1.Initiative).CompareTo(m2.Initiative);
				}));

			// Perform the specified action for each contestant based on initiative.
			// NOTE: Contestants should have chosen targets during the combat action selection.
			for (int i = 0; i < contestants.Length; i++)
			{
				if (contestants[i].CanPerformAction)
				{
					// Find the object defined in the combat action.
					var item = contestants[i].World.GetActor(contestants[i].CombatAction, contestants[i]) as IItem;
					if (item == null)
						continue;

					// Ensure contestant has a target set.
					if (contestants[i].Target == null || !(contestants[i].Target is IAvatar))
						continue;

					if (item is IWeapon)
					{
						var defender = contestants[i].Target as IAvatar;

						// Perform simple combat turn.
						CombatManager.PerformSimpleCombatTurn(
							contestants[i],
							item as IWeapon,
							defender, 
							defender.GetDefensiveSkill(), 
							AttributeType.Dexterity, 
							0);
					}
					else if (item is ISpell)
					{
						MagicManager.PerformCast(item as ISpell, contestants[i], contestants[i].Target);
					}
					else
					{
						// Use the item.
						item.Use(contestants[i], contestants[i].Context);
					}
				}
			}

			// Clear the combat action for each contestant.
			ClearActionResponse(contestants);

			// Determine if the match is complete.
			_isMatchComplete = IsMatchComplete();
			if (_isMatchComplete)
				OnComplete();

			_isPerformingRound = false;
		}

		private void RequestActionResponse(IAvatar[] group)
		{
			for (int i = 0; i < group.Length; i++)
			{
				group[i].Context.Add(new RdlCommand(KnownCommands.CombatAction));
			}
		}

		private bool CheckActionResponse(IAvatar[] group)
		{
			var count = 0;
			for (int i = 0; i < group.Length; i++)
			{
				// If the combat action is not null then the contestant is ready.
				if (!String.IsNullOrEmpty(group[i].CombatAction))
					count++;
			}
			return count == group.Length;
		}

		private void ClearActionResponse(IAvatar[] group)
		{
			for (int i = 0; i < group.Length; i++)
			{
				group[i].CombatAction = String.Empty;
				group[i].Initiative = 0;
			}
		}

		private bool IsMatchComplete()
		{
			// If either all of the attackers or defenders are dead then the match is over.
			var attackerDeadCount = GetDeathCount(_attackers);
			var defenderDeadCount = GetDeathCount(_defenders);

			return attackerDeadCount == _attackers.Length || defenderDeadCount == _defenders.Length;
		}

		private int GetDeathCount(IAvatar[] avatars)
		{
			var count = 0;
			for (int i = 0; i < avatars.Length; i++)
			{
				if (avatars[i].IsDead)
					count++;
			}
			return count;
		}

		private void OnComplete()
		{
			Completed(new CombatMatchCompletedEventArgs { ID = this.ID });
		}
	}

	public delegate void CombatMatchCompletedEventHandler(CombatMatchCompletedEventArgs e);
	public class CombatMatchCompletedEventArgs : EventArgs
	{
		public Guid ID { get; set; }	
	}
}

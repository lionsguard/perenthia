using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	public static class CommandHelper
	{
		#region Drop
		/// <summary>
		/// Removes the current Item from its current Owner and adds the item to the specified target if not null.
		/// </summary>
		/// <param name="item">The current item.</param>
		/// <param name="target">The target in which to drop the current Item. Can be null.</param>
		public static void Drop(this Item item, IActor target)
		{
            // Remove the item from the current owner's collection.
			if (item.Owner != null)
			{
                item.Owner.Children.Remove(item);

                if (item.Owner is Character)
                {
                    (item.Owner as Character).Equipment.Unequip(item);
                    (item.Owner as Character).OnItemDropped(item);
                }
                else
                {
                    // Ensure the item is unequipped during this process.
                    item.IsEquipped = false;
                }
			}

			// Add the item to the current target's collection.
			if (target != null)
			{
				bool addItem = !(target is PerenthiaMobile);
				if (addItem) addItem = !(target is Place);

				if (addItem)
				{
					target.Children.Add(item);
                    if (target is IPlayer)
                    {
                        (target as IPlayer).OnItemReceived(item);
                    }
				}
				else
				{
					item.Owner = null;
				}
			}
            item.Save();
		}

		public static bool Drop(this Item item, IActor giverActor, IActor takerActor, int quantity)
		{
			IAvatar taker = takerActor as IAvatar;
			IAvatar giver = giverActor as IAvatar;
			IActor container = null;

			// Ensure a container exists to recevie the item.
			if (taker != null && taker is Character)
			{
				container = (taker as Character).GetFirstAvailableContainer(item);
			}
			else
			{
				container = takerActor;
			}

			if (container == null)
			{
				if (taker != null) taker.Context.Add(new RdlErrorMessage(Resources.InventoryFull));
				else if (giver != null) giver.Context.Add(new RdlErrorMessage(Resources.CanNotDrop));
				return false;
			}

			// Adjust quantity.
			if (quantity > item.Quantity())
			{
				quantity = item.Quantity();
			}

			// If the specified item has an IsInventoryItem property value of false then immediately
			// USE this item and do not add it to the taker's inventory.
			if (item.IsInventoryItem)
			{
				List<Item> items = new List<Item>();
				if (item.IsStackable && item.Quantity() > 1 && item.Owner != null)
				{
					if (item.Owner is Npc)
					{
						for (int i = 0; i < quantity; i++)
						{
							items.Add(item.Clone() as Item);	
						}
					}
					else
					{
						items = item.Owner.Children.Where(c => c is Item
							&& c.GetType() == item.GetType()
							&& c.Name.Equals(item.Name)).Select(c => c as Item).Take(quantity).ToList();
					}
				}
				else
				{
					items.Add(item);
				}

				// If the container is an actual Container instance then the container must be able to hold
				// all of the available items.
				if (taker is Character)
				{
					for (int i = 0; i < items.Count; i++)
					{
						container = (taker as Character).GetFirstAvailableContainer(items[i]);
						if (container == null)// || !((container as Container).GetRemainingSlots(items[i]) >= quantity))
						{
							if (taker != null) taker.Context.Add(new RdlErrorMessage(Resources.InventoryNotEnoughSpace));
							else if (giver != null)
							{
								giver.Context.Add(new RdlErrorMessage(Resources.CanNotDrop));
								// Send the item back down to the client.
								giver.Context.AddRange(item.ToRdl());
							}
							return false;
						}
					}
				}

				// Add the items to the container.
				for (int i = 0; i < items.Count; i++)
				{
					Item newItem = items[i];
					// If giver is NPC Merchant then clone the templates rather than drop them.
					if (giver is IMerchant)
					{
						newItem = items[i].Clone() as Item;
					}
					
					// Clear the owner and save the object.
					if (newItem.ID == 0)
					{
						newItem.Owner = null;
						newItem.Save();
					}

					// If taker is a Character then different rules apply to where stuff is dropped.
					if (taker is Character)
					{
						Character character = taker as Character;
						if (newItem is ISpell)
						{
							// Spells.Add will add the item to the child collection.
							character.Spells.Add((ISpell)newItem);
							character.Context.AddRange(character.GetRdlProperties(String.Concat(character.Spells.Prefix, character.Spells.Count - 1)));
						}
						else if (newItem is Container && character.Bags.Count < 5)
						{
							character.Bags.Add((Container)newItem);
							character.Context.AddRange(character.GetRdlProperties(String.Concat(character.Bags.Prefix, character.Bags.Count - 1)));
						}
						else
						{
							container = (taker as Character).GetFirstAvailableContainer(newItem);
							if (container != null)
							{
								newItem.Drop(container);
							}
							else
							{
								taker.Context.Add(new RdlErrorMessage(Resources.InventoryNotEnoughSpace));
							}
						}
					}
					else
					{
						newItem.Drop(container);
					}

					if (taker != null) taker.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
						String.Format(Resources.ItemAdded, newItem.AUpper())));

					if (takerActor is Place)
					{
						if (giver != null) giver.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
							String.Format(Resources.ItemDropped, newItem.A())));
					}

					// Send the taker the item and a remove command so the item will be removed from the giver.
					if (taker != null && giver != null) taker.Context.Add(newItem.GetRemoveCommand(giver.ID));
					if (taker != null) taker.Context.AddRange(newItem.ToRdl());

					// Send the giver the item with the new owner and a remove command so the item will be removed.
					if (giver != null) giver.Context.Add(newItem.GetRemoveCommand(giver.ID));
					if (giver != null) giver.Context.AddRange(newItem.ToRdl());
				}
			}
			else
			{
				for (int i = 0; i < quantity; i++)
				{
					// USE the item.
					giver.SetTarget(taker);
					if (item is Spell)
					{
						// Spells need to be cast by the giver onto the taker.
						if (taker != null) item.Use(giver, taker.Context);
					}
					else
					{
						// All other items ned to be used by the taker.
						if (taker != null) item.Use(taker, taker.Context);
					}
				}
			}
			return true;
		}
		#endregion

		#region Inventory
		/// <summary>
		/// Writes the inventory of the current Avatar to the specified IMessageContext.
		/// </summary>
		/// <param name="owner">The Avatar in which to retrieve inventory.</param>
		/// <param name="context">The IMessageContext used to send the inventory details to the client.</param>
		public static void Inventory(this IAvatar owner, IAvatar looker, IMessageContext context)
		{
			// If this is an NPC and a merchant then send down the template instances instead of real items.
			IEnumerable<IItem> items = null;
			if (owner is IMerchant)
			{
				// Return the inventory templates for the current NPC.
				items = (owner as IMerchant).GetGoodsAndServices();

				if (looker != null && owner is Merchant)
				{
					// Set the buy costs for all of the items before sending them down.
					foreach (var item in items)
					{
						Item i = item as Item;
						if (i != null)
						{
							i.BuyCost = i.GetBuyCost(looker, (owner as Merchant).MarkupPercentage);
							i.EmblemBuyCost = i.GetEmblemCost(looker);
						}
					}

					// Then send down the sell costs for all of the inventory items the player has.
					var lookerItems = looker.GetAllChildren().Where(c => c is Item).Select(c => c as Item);
					foreach (var item in lookerItems)
					{
						item.SellCost = item.GetSellCost(looker, (owner as Merchant).MarkdownPercentage);
						item.EmblemSellCost = item.GetEmblemCost(owner);
					}
				}
			}
			else
			{
				items = (from c in owner.GetAllChildren()
						 where (c as IItem) != null
						 select c as IItem);
			}
			if (items != null)
			{
				foreach (var item in items)
				{
					context.AddRange(item.ToRdl());
				}
			}
		}
		#endregion

		#region Attack
		/// <summary>
		/// Causes the user to attack its current target with the current Item as the weapon. If the user's target is a mobile 
		/// a counter attack check will occur and if successful the mobile will counter attack.
		/// </summary>
		/// <param name="weapon">The current item to use as a weapon.</param>
		/// <param name="user">The actor instance using the item as a weapon.</param>
		public static void Attack(this Item weapon, IActor user)
		{
			//if (user is IAvatar)
			//{
			//    if ((user as IAvatar).CombatMatch != Guid.Empty)
			//    {
			//        // Already engaged in combat, ignore the command.
			//    }
			//    else
			//    {
			//        // If the target is already engaged then join the match??
			//    }
			//}

			try
			{
				IAvatar avatar, target;
				IAvatar defender = null;
				IAvatar attacker = null;

				avatar = user as IAvatar;
				if (avatar != null)
				{
					if (avatar.Target != null && avatar.Target is IAvatar)
					{
						target = avatar.Target as IAvatar;

						// If the target is the user then exit.
						if (avatar.ID == target.ID)
						{
							avatar.Context.Add(new RdlErrorMessage(Resources.CanNotAttackSelf));
							return;
						}

						// Target must also have the avatar targeted or be set to null.
						if (target.Target == null)
						{
							target.Target = avatar;
						}
						else if (target.Target.ID != avatar.ID)
						{
							avatar.Context.Add(new RdlErrorMessage(String.Format(Resources.TargetEngaged, target.TheUpper())));
							return;
						}

						// Check for death and unconsioussness.
						if (avatar.IsDead)
						{
							avatar.Context.Add(new RdlErrorMessage(Resources.PlayerDead));
							return;
						}
						if (avatar.IsUnconscious)
						{
							avatar.Context.Add(new RdlErrorMessage(Resources.PlayerUnconscious));
							return;
						}

						if (target.IsDead)
						{
							avatar.Context.Add(new RdlErrorMessage(Resources.TargetDead));
							return;
						}

						// Can not attack certain mobiles, need to check for safe mobiles.
						if (target is PerenthiaMobile && !(target as PerenthiaMobile).CanAttack)
						{
							avatar.Context.Add(new RdlErrorMessage(String.Format(Resources.ActionCanNotPerform, target.A())));
							return;
						}

						// Can not attack a Character unless the IsPvpEnabled is set to true.
						if (target is Character)
						{
							if (avatar is Character && !(target as Character).IsPvpEnabled)
							{
								avatar.Context.Add(new RdlErrorMessage(String.Format(Resources.ActionCanNotPerform, target.A())));
								return;
							}
						}

						// Increase the skill of the player using the item.
						IPlayer currentPlayer = null;
						string skill = null;
						if (avatar is IPlayer)
						{
							currentPlayer = avatar as IPlayer;
							skill = weapon.GetOffensiveSkill();
						}
						else if (target is IPlayer)
						{
							currentPlayer = target as IPlayer;
							skill = target.GetDefensiveSkill();
						}
						if (weapon is Spell)
						{
							skill = weapon.Skill;
						}
						if (currentPlayer != null && !(weapon is Spell))
						{
							// NOTE: Skill advancement occuring in combat and magic manager classes.
							// Advance the skill of the current player.
							//SkillManager.AdvanceSkill(currentPlayer, skill, weapon.SkillLevelRequiredToEquip, currentPlayer.Context);
						}

						bool performCounterAttack = false;
						if (weapon is Spell)
						{
							// CAST A SPELL
							CastResults results = MagicManager.PerformCast((ISpell)weapon, avatar, target);
							if (results.TargetDied)
							{
								attacker = avatar;
								defender = target;
							}
							else
							{
								// If the target is a mobile then counter attack.
								if (!avatar.IsDead && target is IMobile && !target.IsDead && !target.IsUnconscious)
									performCounterAttack = true;
							}
						}
						else
						{
							// USE A WEAPON
							if (CombatManager.PerformSimpleCombatTurn(
								avatar,
								(IWeapon)weapon,
								target,
								target.GetDefensiveSkill(),
								AttributeType.Dexterity,
								0))
							{
								// Killed defender.
								attacker = avatar;
								defender = target;
							}
							else
							{
								// DUAL WIELD
								// If attacker has dual wield skill and an equipped second weapon then
								// perform another attack.
								IWeapon secondaryWeapon = avatar.GetSecondaryWeapon();
								if (secondaryWeapon != null)
								{
									if (CombatManager.PerformSimpleCombatTurn(
										avatar,
										secondaryWeapon,
										"Dual Wield",
										target,
										target.GetDefensiveSkill(),
										AttributeType.Dexterity,
										0))
									{
										// Killed defender.
										attacker = avatar;
										defender = target;
									}
									else
									{
										// Defender counter attack.
										// Only counter if a mobile.
										if (target is IMobile && !target.IsDead && !target.IsUnconscious)
											performCounterAttack = true;
									}
								}
								else
								{
									// Defender counter attack.
									// Only counter if a mobile.
									if (target is IMobile && !target.IsDead && !target.IsUnconscious)
										performCounterAttack = true;
								}
							}
						}

						if (performCounterAttack)
						{
							if (!target.IsStunned && !target.IsFrozen && !avatar.IsDead)
							{
								// TODO: Have mobiles attack with spells??
								// USE A WEAPON
								if (CombatManager.PerformSimpleCombatTurn(
									target,
									target.GetWeapon(),
									avatar,
									avatar.GetDefensiveSkill(),
									AttributeType.Dexterity,
									0))
								{
									// Killed attacker.
									attacker = target;
									defender = avatar;
								}

								// Update the mobile's last attack time.
								(target as PerenthiaMobile).LastAttackTime = DateTime.Now;
							}
						}

						// Send down stats for both attacker and defender.
						var attackerProps = avatar.GetRdlProperties(Avatar.BodyProperty, Avatar.MindProperty);
						var defenderProps = target.GetRdlProperties(Avatar.BodyProperty, Avatar.MindProperty);
						avatar.Context.AddRange(attackerProps);
						avatar.Context.AddRange(defenderProps);
						target.Context.AddRange(attackerProps);
						target.Context.AddRange(defenderProps);

						if (defender != null && attacker != null)
						{
							if (attacker is IPlayer)
							{
								//=========================================================================================
								// PLAYER KILLED MOBILE
								//=========================================================================================
								// The mobile dies, cause it to respawn at a later date.
								PerenthiaMobile mob = defender as PerenthiaMobile;
								if (mob != null)
								{
									// Mobile is a clone but is stored as an instance on the mobile object.
									DateTime killTime = DateTime.Now;
									mob.RespawnTime = killTime.Add(mob.RespawnDelay);
									mob.KilledBy.AddKilledBy(attacker.ID, killTime);
								}
								attacker.Context.AddRange(defender.ToSimpleRdl());

								// Since the player killed the mobile, give them some experience.
								Character player = attacker as Character;
								if (player != null && mob != null)
								{
									int xp = LevelManager.GetXpForMobileKill(player.Level, mob.Level);
									if (xp > 0)
									{
										player.Experience += xp;
										player.TotalExperience += xp;
										player.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Positive,
											String.Format(Resources.ExperienceGained, xp)));
									}

									// Raise an event indicating that the player has killed something.
									player.World.OnAvatarKilledAvatar(player, mob);

									// Raise an internal event on the player instance for quests and awards.
									player.OnKilledActor(mob);

									// Cause the player to advance if the required experience requirements are met.
									LevelManager.AdvanceIfAble(player);

									// Mobiles should drop random items, mostly items related to crafting.
									mob.GenerateRandomDropItems();

									// Send down changed properties.
									player.Context.AddRange(player.GetRdlProperties(
										Character.ExperienceProperty,
										Character.ExperienceMaxProperty,
										Character.CurrencyProperty));

									// Save the player instance.
									player.Save();
								}
							}
							else if (defender is IPlayer)
							{
								//=========================================================================================
								// MOBILE KILLED PLAYER
								//=========================================================================================
								// A player died, dock some experience.
								// Only loose experience after level 5.
								Character player = defender as Character;
								if (player != null && player.Experience > 0 && player.Level > 5)
								{
									int penaltyXp = player.Level * 50 + (Dice.Roll(player.Level, 10));
									if (penaltyXp > player.Experience) penaltyXp = player.Experience - 2;
									player.Experience -= penaltyXp;
									player.TotalExperience -= penaltyXp;
									player.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.Negative,
										String.Format(Resources.ExperienceLost, penaltyXp)));

									// Send down changed properties.
									player.Context.AddRange(player.GetRdlProperties(
										Character.ExperienceProperty,
										Character.ExperienceMaxProperty));
								}

								// Raise an event on the player for quests and awards.
								if (defender != null) player.OnDied(defender);

								if (attacker != null && defender != null)
								{
									// Raise an event indicating that the defender has killed the attacker.
									defender.World.OnAvatarKilledAvatar(attacker, defender);
								}

								// Find the nearest temple and resurrect the player there.
								Temple temple = Game.FindTemple(defender.Location);
								if (temple != null)
								{
									temple.Resurrect(defender);
								}
								else
								{
									// Otherwise, spaw a temple right above the player's starting location and send them there.
									Race race = defender.World.Races[defender.Race];
									Temple newTemple = new Temple()
									{
										Location = new Point3(race.StartingLocation.X, race.StartingLocation.Y, race.StartingLocation.Z + 1),
										Name = "Temple",
										World = defender.World,
										Terrain = defender.World.Terrain[1].ID,
									};
									newTemple.Exits.SetValue(KnownDirection.Down, true);
									defender.World.Places.Add(newTemple.Location, newTemple);
									Game.AddTemple(newTemple);
								}

								// Save the player instance.
								defender.Save();
							}
							// Do not reset targets after combat as the target might be clone of a mob and will need
							// to remain set for proper looting.
							// Reset targets.
							//attacker.Target = null;
							//defender.Target = null;
							//attacker.Context.AddRange(attacker.GetRdlProperties(Avatar.TargetIDProperty));
							//defender.Context.AddRange(defender.GetRdlProperties(Avatar.TargetIDProperty));
						}
					}
					else
					{
						avatar.Context.Add(new RdlErrorMessage(String.Format("You must have a target selected to use {0}.", weapon.The())));
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		#endregion

		#region Target
		public static void SetTarget(this IAvatar avatar, IAvatar target)
		{
			// If the target's target is not null then check to see if the target's target is in the current
			// place, if not release the target.
			// Will not need to do this because mobiles are cloned.
			//IAvatar targetTarget = target.Target as IAvatar;
			//if (targetTarget != null)
			//{
			//    if (targetTarget.Location != target.Location)
			//    {
			//        target.Target = null;
			//    }
			//}

			avatar.Target = target;
			if (target != null)
			{
				target.Target = avatar;

				// Send notification to avatar.
				avatar.Context.AddRange(avatar.GetRdlProperties(Avatar.TargetIDProperty));
				avatar.Context.AddRange(target.ToSimpleRdl());
				avatar.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
					String.Format(Resources.TargetSet, target.A())));

				// Send notification to target
				target.Context.AddRange(target.GetRdlProperties(Avatar.TargetIDProperty));
				target.Context.AddRange(avatar.ToSimpleRdl());
				// Do not send a message to the target, not really needed.
				//target.Context.Add(new RdlSystemMessage(RdlSystemMessage.PriorityType.None,
				//    String.Format(Resources.TargetSet, avatar.A())));
			}
		}
		#endregion
	}
}

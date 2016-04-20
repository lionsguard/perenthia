using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Radiance;
using Radiance.Markup;

namespace Perenthia
{
	#region Sentinal
	public class Sentinal : Npc
	{
		public Sentinal()
			: base("Sentinal", String.Empty, 1, 4, 6, MobileTypes.Sentinal, new FighterSkillGroup())
		{
		}

		public Sentinal(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Sentinal, new FighterSkillGroup())
		{
		}
	}
	#endregion

	#region Guard
	public class Guard : Npc
	{
		public Guard()
			: base("Guard", String.Empty, 1, 4, 6, MobileTypes.Guard, new FighterSkillGroup())
		{
		}

		public Guard(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Guard, new FighterSkillGroup())
		{
		}
	}
	#endregion

	#region Merchant
	public class Merchant : Npc, IMerchant
	{
		public TemplateItemCollection SellableItems { get; private set; }

		public DateTime LastMerchandisePurgeDate
		{	
			get { return this.Properties.GetValue<DateTime>(LastMerchandisePurgeDateProperty); }
			set { this.Properties.SetValue(LastMerchandisePurgeDateProperty, value, false); }
		}
		public static readonly string LastMerchandisePurgeDateProperty = "LastMerchandisePurgeDate";

		public Merchant()
			: base("Merchant", String.Empty, 1, 4, 6, MobileTypes.Merchant, new FighterSkillGroup())
		{
		}

		public Merchant(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Merchant, new FighterSkillGroup())
		{
		}

		public Merchant(string name, string description, int level, int attributeMin, int attributeMax, MobileTypes type, SkillGroup skills)
			: base(name, description, level, attributeMin, attributeMax, type, skills)
		{
		}

		protected override void Init()
		{
			base.Init();
			this.LastMerchandisePurgeDate = DateTime.Now;
			this.SellableItems = new TemplateItemCollection(this, "SellableItem_");
		}

		public virtual void AddMerchandise(IItem item)
		{
			this.SellableItems.Add(new TemplateItem { Name = item.Name, Quantity = 1 });
		}

		public virtual IEnumerable<IItem> GetGoodsAndServices()
		{
			// Once a day perge the merchant inventory.
			TimeSpan remainder = this.LastMerchandisePurgeDate.Subtract(DateTime.Now);
			if (remainder.TotalHours > 2)
			{
				// Purge all items not found in the sellable items collection.
				var items = this.GetAllChildren().Where(c => c is IItem).Select(c => c as IItem)
					.Where(i => !this.SellableItems.ContainsItem(i.Name));
				foreach (var item in items)
				{
					item.Owner = null;
					item.Save();
				}
				this.LastMerchandisePurgeDate = DateTime.Now;
				this.Save();
			}

			IEnumerable<IItem> children = this.GetAllChildren().Where(c => c is IItem).Select(c => c as IItem);
			// Ensure all the items specified in sellableitems exist in the inventory of the merchant.
			foreach (var t in this.SellableItems)
			{
				IItem item = children.Where(i => i.Name == t.Name).FirstOrDefault();
				if (item == null)
				{
					item = this.World.CreateFromTemplate<IItem>(t.Name);
					if (item != null)
					{
						this.Children.Add(item);
						item.Save();
					}
				}
			}
			return this.GetAllChildren().Where(c => c is IItem).Select(c => c as IItem);
		}
	}
	#endregion

	#region InnKeeper
	public class InnKeeper : Merchant
	{
		public InnKeeper()
			: base("InnKeeper", String.Empty, 1, 4, 6, MobileTypes.Innkeeper, new FighterSkillGroup())
		{
		}

		public InnKeeper(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Innkeeper, new FighterSkillGroup())
		{
		}
	}
	#endregion

	#region Banker
	public class Banker : Merchant
	{
		public Banker()
			: base("Banker", String.Empty, 1, 4, 6, MobileTypes.Banker, new FighterSkillGroup())
		{
		}

		public Banker(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Banker, new FighterSkillGroup())
		{
		}
	}
	#endregion

	#region Trainer
	public class Trainer : Merchant
	{
		public Trainer()
			: base("Trainer", String.Empty, 1, 4, 6, MobileTypes.Trainer, new FighterSkillGroup())
		{
		}

		public Trainer(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Trainer, new FighterSkillGroup())
		{
		}
	}
	#endregion

	#region QuestGiver
	public class QuestGiver : Npc
	{
		public QuestGiver()
			: base("QuestGiver", String.Empty, 1, 4, 6, MobileTypes.QuestGiver, new FighterSkillGroup())
		{
		}

		public QuestGiver(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.QuestGiver, new FighterSkillGroup())
		{
		}
	}
	#endregion

	#region Priest
	public class Priest : Merchant
	{
		public Priest()
			: base("Priest", String.Empty, 1, 4, 6, MobileTypes.Priest, new CasterSkillGroup())
		{
		}

		public Priest(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Priest, new CasterSkillGroup())
		{
		}

		public override void OnCastSkillRoll(ISpell spell, ref int castSuccessCount)
		{
			if (castSuccessCount <= 0) castSuccessCount = 1;
		}

		public override void OnCastSuccess(ISpell spell)
		{
			this.SetMind(this.MindMax);
		}
	}
	#endregion

	#region Roamer
	public class Roamer : Npc
	{
		public Roamer()
			: base("Roamer", String.Empty, 1, 4, 6, MobileTypes.Roamer, new FighterSkillGroup())
		{
		}

		public Roamer(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Roamer, new FighterSkillGroup())
		{
		}
	}
	#endregion

	#region Repairer
	public class Repairer : Merchant
	{
		public Repairer()
			: base("Repairer", String.Empty, 1, 4, 6, MobileTypes.Repairer, new CasterSkillGroup())
		{
		}

		public Repairer(string name, string description, int level, int attributeMin, int attributeMax)
			: base(name, description, level, attributeMin, attributeMax, MobileTypes.Repairer, new CasterSkillGroup())
		{
		}
	}
	#endregion
}

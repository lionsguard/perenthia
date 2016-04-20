using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using Lionsguard;
using Radiance;
using Radiance.Markup;

namespace Perenthia.Controls
{
	public partial class ItemSlot : UserControl, IDropContainer, IDroppable
	{
		public static event ItemSlotEventHandler SlotClick = delegate { };

		public event ItemSlotEventHandler Click = delegate { };

		private bool _isMouseOver = false;
		private bool _isPressed = false;
		private bool _showQuantity = false;

		private ContextMenu _context = null;

		public int Quantity
		{
			get { return (int)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}
		public static readonly DependencyProperty QuantityProperty = DependencyProperty.Register("Quantity", typeof(int), typeof(ItemSlot), new PropertyMetadata(0, new PropertyChangedCallback(ItemSlot.OnQuantityPropertyChanged)));
		private static void OnQuantityPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ItemSlot slot = obj as ItemSlot;
			if (slot != null)
			{
				slot._showQuantity = slot.Quantity > 1;
				if (slot._showQuantity)
				{
					slot.QuantityLabelElement.Text = slot.Quantity.ToString();
					slot.QuantityLabelElement.Visibility = Visibility.Visible;
				}
				else
				{
					slot.QuantityLabelElement.Visibility = Visibility.Collapsed;
				}
				slot.GoToState(true);
			}
		}

		public bool Selected
		{
			get { return (bool)GetValue(SelectedProperty); }
			set { SetValue(SelectedProperty, value); }
		}
		public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(ItemSlot), new PropertyMetadata(false, new PropertyChangedCallback(ItemSlot.OnSelectedPropertyChanged)));
		private static void OnSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ItemSlot).GoToState(true);
		}

		public ImageSource BackgroundSource
		{
			get { return (ImageSource)GetValue(BackgroundSourceProperty); }
			set { SetValue(BackgroundSourceProperty, value); }
		}
		public static readonly DependencyProperty BackgroundSourceProperty = DependencyProperty.Register("BackgroundSource", typeof(ImageSource), typeof(ItemSlot), new PropertyMetadata(null, new PropertyChangedCallback(ItemSlot.OnBackgroundSourcePropertyChanged)));
		private static void OnBackgroundSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ItemSlot).BackgroundElement.Source = (obj as ItemSlot).BackgroundSource;
		}

		public ImageSource Source
		{
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(ItemSlot), new PropertyMetadata(null, new PropertyChangedCallback(ItemSlot.OnSourcePropertyChanged)));
		private static void OnSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ItemSlot).IconElement.Source = (obj as ItemSlot).Source;
		}

		public RdlActor Item
		{
			get { return (RdlActor)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(RdlActor), typeof(ItemSlot), new PropertyMetadata(null, new PropertyChangedCallback(ItemSlot.OnItemPropertyChanged)));
		private static void OnItemPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ItemSlot).Refresh();
		}

		public ItemSlotType SlotType
		{
			get { return (ItemSlotType)GetValue(SlotTypeProperty); }
			set { SetValue(SlotTypeProperty, value); }
		}
		public static readonly DependencyProperty SlotTypeProperty = DependencyProperty.Register("SlotType", typeof(ItemSlotType), typeof(ItemSlot), new PropertyMetadata(ItemSlotType.Item, new PropertyChangedCallback(ItemSlot.OnSlotTypePropertyChanged)));
		private static void OnSlotTypePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{	
		}

		public bool EnableDragAndDrop
		{
			get { return (bool)GetValue(EnableDragAndDropProperty); }
			set { SetValue(EnableDragAndDropProperty, value); }
		}
		public static readonly DependencyProperty EnableDragAndDropProperty = DependencyProperty.Register("EnableDragAndDrop", typeof(bool), typeof(ItemSlot), new PropertyMetadata(false));

		public EquipmentSlot EquipmentSlot
		{
			get { return (EquipmentSlot)GetValue(EquipmentSlotProperty); }
			set { SetValue(EquipmentSlotProperty, value); }
		}
		public static readonly DependencyProperty EquipmentSlotProperty = DependencyProperty.Register("EquipmentSlot", typeof(EquipmentSlot), typeof(ItemSlot), new PropertyMetadata(EquipmentSlot.None));

		public int ActionSlotNumber
		{
			get { return (int)GetValue(ActionSlotNumberProperty); }
			set { SetValue(ActionSlotNumberProperty, value); }
		}
		public static readonly DependencyProperty ActionSlotNumberProperty = DependencyProperty.Register("ActionSlotNumber", typeof(int), typeof(ItemSlot), new PropertyMetadata(0));

		public string ToolTip
		{
			get { return (string)GetValue(ToolTipProperty); }
			set { SetValue(ToolTipProperty, value); }
		}
		public static readonly DependencyProperty ToolTipProperty = DependencyProperty.Register("ToolTip", typeof(string), typeof(ItemSlot), new PropertyMetadata(null, new PropertyChangedCallback(ItemSlot.OnToolTipPropertyChanged)));
		private static void OnToolTipPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			ToolTipService.SetToolTip(obj, e.NewValue);
		}

		public bool EnabledMenu
		{
			get { return (bool)GetValue(EnabledMenuProperty); }
			set { SetValue(EnabledMenuProperty, value); }
		}
		public static readonly DependencyProperty EnabledMenuProperty = DependencyProperty.Register("EnabledMenu", typeof(bool), typeof(ItemSlot), new PropertyMetadata(true, new PropertyChangedCallback(ItemSlot.OnEnabledMenuPropertyChanged)));
		private static void OnEnabledMenuPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as ItemSlot).Refresh();
		}   

        public int OwnerID
        {
            get { return (int)GetValue(OwnerIDProperty); }
            set { SetValue(OwnerIDProperty, value); }
        }
        public static readonly DependencyProperty OwnerIDProperty = DependencyProperty.Register("OwnerID", typeof(int), typeof(ItemSlot), new PropertyMetadata(0));

		public ItemSlot()
		{
			InitializeComponent();
		}

		#region Event Handlers
		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			ToolTipService.SetToolTip(MenuElement, "Actions...");
			this.GoToState(true);
			if (this.Item != null)
			{
				this.Refresh();
			}
			else
			{
				this.Quantity = 0;
				this.UseBorderElement.Visibility = Visibility.Collapsed;
				this.MenuElement.Visibility = Visibility.Collapsed;
			}
		}

		private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_isPressed = true;
			this.GoToState(true);

			if (this.SlotType == ItemSlotType.Action)
			{
				Use();
			}
			else
			{
				// Need a better way to handle this.
				if (this.EnableDragAndDrop && Game.DragDropManager != null)
				{
					e.Handled = true;
					Game.DragDropManager.BeginDrag(this, e.GetPosition(null));
				}
				else
				{
					ItemSlotEventArgs args = new ItemSlotEventArgs(this);
					this.Click(args);
					if (!args.Handled)
					{
						ItemSlot.SlotClick(new ItemSlotEventArgs(this));
					}
				}
			}
		}

		private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isPressed = false;
			this.GoToState(true);
		}

		private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
		{
			_isMouseOver = true;
			this.GoToState(true);
		}

		private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
		{
			_isMouseOver = false;
			this.GoToState(true);
		}

		private void MenuElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			if (this.Item != null)
			{
				ContextMenu.ContextMenuType cmType = ContextMenu.ContextMenuType.Item;
				switch (this.SlotType)
				{	
					case ItemSlotType.Equipment:
						cmType = ContextMenu.ContextMenuType.Equipment;
						break;
					case ItemSlotType.Action:
						cmType = ContextMenu.ContextMenuType.Action;
						break;
				}
				_context = new ContextMenu(cmType, this.Item);
				_context.Click += new ActionEventHandler(OnContextMenuClick);
                _context.CreateAction += new CreateActionEventHandler(OnContextCreateAction);
				_context.MouseLeave += new MouseEventHandler(OnContextMouseLeave);

				PopupManager.Add(_context, e.GetPosition(null));
			}
		}

        private ActionEventArgs OnContextCreateAction(CreateActionEventArgs e)
        {
            ActionEventArgs args = null;
            switch (e.ActionName)
            {
                case Actions.SetAction:
                    // Clears the action. (Dropping an action sets the action, the menu is used to clear it)
                    // SETACTION Args: 0 = ItemID (e.Actor.ActorAlias, 1 = SlotNumber
                    args = new ActionEventArgs(e.ActionName, 0, String.Empty, this.ActionSlotNumber);
                    break;
            }
            return args;
        }

		private void OnContextMouseLeave(object sender, MouseEventArgs e)
		{
			// Just hide the context if the mouse leaves the area of the context menu.
			this.HideContext();
		}

		private void OnContextMenuClick(object sender, ActionEventArgs e)
		{
			// Execute the action.
			this.ExecuteAction(e);

			// Hide the context control.
			this.HideContext();
		}

		private void HideContext()
		{
			if (_context != null)
			{
                _context.Click -= new ActionEventHandler(OnContextMenuClick);
                _context.CreateAction -= new CreateActionEventHandler(OnContextCreateAction);
				_context.MouseLeave -= new MouseEventHandler(OnContextMouseLeave);
				_context = null;
			}
			PopupManager.Remove();
		}
		#endregion

		#region Refresh
		private void Refresh()
		{
			if (this.Item != null)
			{
				if (this.EnabledMenu)
				{
					this.UseBorderElement.Visibility = Visibility.Visible;
					this.MenuElement.Visibility = Visibility.Visible;
				}
				else
				{
					this.UseBorderElement.Visibility = Visibility.Collapsed;
					this.MenuElement.Visibility = Visibility.Collapsed;
				}

				// If this slot type is item or equipment then enable drag and drop.
				if (this.SlotType == ItemSlotType.Item 
                    || this.SlotType == ItemSlotType.Equipment
                    || this.SlotType == ItemSlotType.Spell)
				{
					this.EnableDragAndDrop = true;
				}

				this.Source = ImageManager.GetImageSource(this.Item.ImageUri());
				this.Quantity = this.Item.Quantity();
				ToolTipService.SetToolTip(this, String.Format("{0}", this.Item.Name));
			}
			else
			{
				ToolTipService.SetToolTip(this, this.ToolTip);
				this.Source = null;
				this.Quantity = 0;
				this.UseBorderElement.Visibility = Visibility.Collapsed;
				this.MenuElement.Visibility = Visibility.Collapsed;
			}
		}
		#endregion

		#region GoToState
		private void GoToState(bool useTransitions)
		{
			// Common States
			if (_isPressed)
			{
				VisualStateManager.GoToState(this, "Pressed", useTransitions);
			}
			else if (_isMouseOver)
			{
				VisualStateManager.GoToState(this, "MouseOver", useTransitions);
			}
			else
			{
				VisualStateManager.GoToState(this, "Normal", useTransitions);
			}

			// Slot States
			if (this.Selected)
			{
				if (_showQuantity)
				{
					VisualStateManager.GoToState(this, "SelectedShowQuantity", useTransitions);
				}
				else
				{
					VisualStateManager.GoToState(this, "SelectedHideQuantity", useTransitions);
				}
			}
			else
			{
				if (_showQuantity)
				{
					VisualStateManager.GoToState(this, "UnselectedShowQuantity", useTransitions);
				}
				else
				{
					VisualStateManager.GoToState(this, "UnselectedHideQuantity", useTransitions);
				}
			}
		}
		#endregion

		#region IDropContainer Members

		public void Drop(IDroppable item)
		{
			if (item is ItemSlot)
			{
				ItemSlot slot = (ItemSlot)item;
				switch (this.SlotType)
				{	
					case ItemSlotType.Item:
                        // Requires an owner id to drop on item slot.
                        if (this.OwnerID > 0)
                        {
                            // Find an open slot and drop the item.
                            if (slot.EquipmentSlot != EquipmentSlot.None && slot.Item.IsEquipped())
                            {
                                // Unequip the item.
                                slot.Item.OwnerID = this.OwnerID;
                                slot.Item.Properties.SetValue("IsEquipped", false);
                                ServerManager.Instance.SendCommand(new RdlCommand("UNEQUIP", slot.Item.ID, slot.EquipmentSlot, this.OwnerID));
                            }
                            else
                            {
                                ServerManager.Instance.SendCommand(new RdlCommand("GIVE", slot.Item.ID, this.OwnerID, 1));
                            }
                        }
						break;
					case ItemSlotType.Equipment:
						// Can only drop items or equipment.
						if (slot.SlotType == ItemSlotType.Item || slot.SlotType == ItemSlotType.Equipment)
						{
							if (slot.Item.EquipLocation() != EquipLocation.None)
							{
								if (this.Item != null)
								{
									if (this.Item.IsEquipped())
									{
										ServerManager.Instance.SendCommand("UNEQUIP", slot.Item.ID, slot.EquipmentSlot);
									}
								}
								ServerManager.Instance.SendCommand("EQUIP", slot.Item.ID, slot.EquipmentSlot);
							}
						}
						break;
					case ItemSlotType.Action:
						// Can drop items.
						if (slot.Item != null)
						{
							// Set the current item into the specified action slot for this player but leave the item in the slot
							// it was dragged from.
							Game.Player.Properties.SetValue(String.Concat("Action_", this.ActionSlotNumber), slot.Item.ID);
							ServerManager.Instance.SendCommand(new RdlCommand("SETACTION", slot.Item.ID, this.ActionSlotNumber));

							// Set the dragged item into the specified slot.
							this.Item = slot.Item;
						}
						break;
					case ItemSlotType.Key:
						// Can drop items that are keys.
						break;
					case ItemSlotType.Bag:
                        if (slot.Item != null)
                        {
                            // This is a bag, need to drop the item into the bag.
                            ServerManager.Instance.SendCommand(new RdlCommand("GIVE", slot.Item.ID, this.Item.ID, 1));
                            slot.Item = null;
                        }
						break;
					case ItemSlotType.Spell:
						// Requires an owner id.
                        if (slot.Item != null && slot.Item.EquipLocation() == EquipLocation.Spell)
                        {
                            slot.Item.Properties.SetValue("IsEquipped", true);
                            ServerManager.Instance.SendCommand(new RdlCommand("EQUIP", slot.Item.ID, EquipmentSlot.None));
                            this.Item = slot.Item;
                        }
						break;
				}
			}
		}

		#endregion

		#region IDroppable Members

		public event BeginDragEventHandler BeginDrag = delegate { };

		public UIElement GetDragCursor()
		{
			if (this.Item != null)
			{
				Image img = new Image();
				img.Source = this.Source;
				img.Width = img.Height = 32;
				return img;
			}
			return new Image();
		}

		#endregion

		public void Use()
		{
			if (this.Item != null)
			{
				this.ExecuteAction(new ActionEventArgs(Actions.Use, this.Item.ID, this.Item.Name));
			}
		}

		private void ExecuteAction(ActionEventArgs e)
		{
			// Execute the action.
			RdlCommand cmd = new RdlCommand(e.ActionName);
			cmd.Args.Add(e.ActorAlias);
			cmd.Args.AddRange(e.Args.ToArray());

			ServerManager.Instance.SendCommand(cmd);
		}
	}

	public enum ItemSlotType
	{
		Item = 0, 
		Equipment = 1,
		Action = 2, 
		Key = 3,
		Bag = 4,
		Spell = 5,
		Menu = 6,
	}

	public delegate	void ItemSlotEventHandler(ItemSlotEventArgs e);
	public class ItemSlotEventArgs : EventArgs
	{
		public ItemSlot Slot { get; set; }
		public bool Handled { get; set; }	

		public ItemSlotEventArgs(ItemSlot slot)
		{
			this.Slot = slot;
		}
	}
}

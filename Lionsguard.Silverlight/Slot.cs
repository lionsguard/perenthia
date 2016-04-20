using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Lionsguard
{
	/// <summary>
	/// Represents a slot for storing icons and items such as backpack cells.
	/// </summary>
	[TemplatePart(Name = "RootElement", Type = typeof(FrameworkElement))]
	[TemplatePart(Name = "BackgroundElement", Type = typeof(Image))]
	[TemplatePart(Name = "IconElement", Type = typeof(Image))]
	[TemplatePart(Name = "BorderElement", Type = typeof(Border))]
	[TemplatePart(Name = "MouseOverVisualElement", Type = typeof(Border))]
	[TemplatePart(Name = "SelectedVisualElement", Type = typeof(Border))]
	[TemplatePart(Name = "QuantityBorderElement", Type = typeof(Border))]
	[TemplatePart(Name = "QuantityLabelElement", Type = typeof(TextBlock))]
	[TemplatePart(Name = "UseElement", Type = typeof(Path))]
	[TemplatePart(Name = "DropElement", Type = typeof(TextBlock))]
	[TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "MouseOver", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Pressed", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
	[TemplateVisualState(Name = "UnselectedShowQuantity", GroupName = "SlotStates")]
	[TemplateVisualState(Name = "SelectedShowQuantity", GroupName = "SlotStates")]
	[TemplateVisualState(Name = "UnselectedHideQuantity", GroupName = "SlotStates")]
	[TemplateVisualState(Name = "SelectedHideQuantity", GroupName = "SlotStates")]
	public class Slot : Control
	{
		#region Properties
		/// <summary>
		/// Gets the root FrameworkElement of the control.
		/// </summary>
		public FrameworkElement RootElement { get; private set; }
		private Image BackgroundElement { get; set; }
		private Image IconElement { get; set; }
		private Border BorderElement { get; set; }
		private Border MouseOverVisualElement { get; set; }
		private Border SelectedVisualElement { get; set; }
		private Border QuantityBorderElement { get; set; }
		private TextBlock QuantityLabelElement { get; set; }
		private Path UseElement { get; set; }
		private TextBlock DropElement { get; set; }

		private bool _isMouseOver = false;
		private bool _isPressed = false;
		private bool _showQuantity = false;

		/// <summary>
		/// Identifies the Lionsguard.Slot.Selected property.
		/// </summary>
		public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register("Selected", typeof(bool), typeof(Slot), new PropertyMetadata(new PropertyChangedCallback(Slot.OnSelectedPropertyChanged)));
		/// <summary>
		/// Gets or sets a value indicating whether or not the item is in the slot is selected.
		/// </summary>
		public bool Selected
		{
			get { return (bool)this.GetValue(SelectedProperty); }
			set { this.SetValue(SelectedProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.BackgroundSource property.
		/// </summary>
		public static readonly DependencyProperty BackgroundSourceProperty = DependencyProperty.Register("BackgroundSource", typeof(ImageSource), typeof(Slot), new PropertyMetadata(new PropertyChangedCallback(Slot.OnBackgroundSourcePropertyChanged)));
		/// <summary>
		/// Gets or sets the source of the faded background image of the slot.
		/// </summary>
		public ImageSource BackgroundSource
		{
			get { return (ImageSource)this.GetValue(BackgroundSourceProperty); }
			set { this.SetValue(BackgroundSourceProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.Source property.
		/// </summary>
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(ImageSource), typeof(Slot), new PropertyMetadata(new PropertyChangedCallback(Slot.OnSourcePropertyChanged)));
		/// <summary>
		/// Gets or sets the primary image of the slot.
		/// </summary>
		public ImageSource Source
		{
			get { return (ImageSource)this.GetValue(SourceProperty); }
			set { this.SetValue(SourceProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.SelectedBorderBrush property.
		/// </summary>
		public static readonly DependencyProperty SelectedBorderBrushProperty = DependencyProperty.Register("SelectedBorderBrush", typeof(Brush), typeof(Slot), null);
		/// <summary>
		/// Gets or sets the brush for the slot border.
		/// </summary>
		public Brush SelectedBorderBrush
		{
			get { return (Brush)this.GetValue(SelectedBorderBrushProperty); }
			set { this.SetValue(SelectedBorderBrushProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.SelectedBackground property.
		/// </summary>
		public static readonly DependencyProperty SelectedBackgroundProperty = DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(Slot), null);
		/// <summary>
		/// Gets or sets the brush used to paint the background of the slot while selected.
		/// </summary>
		public Brush SelectedBackground
		{
			get { return (Brush)this.GetValue(SelectedBackgroundProperty); }
			set { this.SetValue(SelectedBackgroundProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.MouseOverBrush property.
		/// </summary>
		public static readonly DependencyProperty MouseOverBrushProperty = DependencyProperty.Register("MouseOverBrush", typeof(Brush), typeof(Slot), null);
		/// <summary>
		/// Gets or sets the brush used to paint the background of the slot during mouse hover.
		/// </summary>
		public Brush MouseOverBrush
		{
			get { return (Brush)this.GetValue(MouseOverBrushProperty); }
			set { this.SetValue(MouseOverBrushProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.QuantityBrush property.
		/// </summary>
		public static readonly DependencyProperty QuantityBrushProperty = DependencyProperty.Register("QuantityBrush", typeof(Brush), typeof(Slot), null);
		/// <summary>
		/// Gets or sets the brush used to paint the foreground of the quantity text.
		/// </summary>
		public Brush QuantityBrush
		{
			get { return (Brush)GetValue(QuantityBrushProperty); }
			set { SetValue(QuantityBrushProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.UseBrush property.
		/// </summary>
		public static readonly DependencyProperty UseBrushProperty = DependencyProperty.Register("UseBrush", typeof(Brush), typeof(Slot), null);
		/// <summary>
		/// Gets or sets the brush used to paint the USE button element.
		/// </summary>
		public Brush UseBrush
		{
			get { return (Brush)GetValue(UseBrushProperty); }
			set { SetValue(UseBrushProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.DropBrush property.
		/// </summary>
		public static readonly DependencyProperty DropBrushProperty = DependencyProperty.Register("DropBrush", typeof(Brush), typeof(Slot), null);
		/// <summary>
		/// Gets or sets the brush used to paint the DROP button element.
		/// </summary>
		public Brush DropBrush
		{
			get { return (Brush)GetValue(DropBrushProperty); }
			set { SetValue(DropBrushProperty, value); }
		}
			
		/// <summary>
		/// Identifies the Lionsguard.Slot.Quantity property.
		/// </summary>
		public static readonly DependencyProperty QuantityProperty = DependencyProperty.Register("Quantity", typeof(int), typeof(Slot), new PropertyMetadata(0, new PropertyChangedCallback(Slot.OnQuantityPropertyChanged)));
		/// <summary>
		/// Gets or sets the quantity of the item hosted in the slot and will display over the source image in the corner of the slot.
		/// </summary>
		public int Quantity
		{
			get { return (int)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.EnableUse property.
		/// </summary>
		public static readonly DependencyProperty EnableUseProperty = DependencyProperty.Register("EnableUse", typeof(bool), typeof(Slot), new PropertyMetadata(true));
		/// <summary>
		/// Gets or sets a value indicating whether or not the USE command is enabled for this slot. Default is true.
		/// </summary>
		public bool EnableUse
		{
			get { return (bool)GetValue(EnableUseProperty); }
			set { SetValue(EnableUseProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.EnableDrop property.
		/// </summary>
		public static readonly DependencyProperty EnableDropProperty = DependencyProperty.Register("EnableDrop", typeof(bool), typeof(Slot), new PropertyMetadata(true));
		/// <summary>
		/// Gets or sets a value indicating whether or not the DROP command is enabled for this slot. Default is true.
		/// </summary>
		public bool EnableDrop
		{
			get { return (bool)GetValue(EnableDropProperty); }
			set { SetValue(EnableDropProperty, value); }
		}
			
		/// <summary>
		/// Identifies the Lionsguard.Slot.IsDraggable property.
		/// </summary>
		public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.Register("IsDraggable", typeof(bool), typeof(Slot), new PropertyMetadata(true, new PropertyChangedCallback(Slot.OnIsDraggablePropertyChanged)));
		/// <summary>
		/// Gets or sets a value indicating whether or not the Item associated with the current slot can be dragged into other slots or containers.
		/// </summary>
		public bool IsDraggable
		{
			get { return (bool)GetValue(IsDraggableProperty); }
			set { SetValue(IsDraggableProperty, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.IsDroppable property.
		/// </summary>
		public static readonly DependencyProperty IsDroppableProperty = DependencyProperty.Register("IsDroppable", typeof(bool), typeof(Slot), new PropertyMetadata(true, new PropertyChangedCallback(Slot.OnIsDroppablePropertyChanged)));
		/// <summary>
		/// Gets or sets a value indicating whether or not the Item associated with the current slot can accept drops from other containers.
		/// </summary>
		public bool IsDroppable
		{
			get { return (bool)GetValue(IsDroppableProperty); }
			set { SetValue(IsDroppableProperty, value); }
		}
			
		/// <summary>
		/// Gets or sets the ToolTip for the current slot.
		/// </summary>
		public object ToolTip
		{
			get { return ToolTipService.GetToolTip(this); }
			set { ToolTipService.SetToolTip(this, value); }
		}

		/// <summary>
		/// Identifies the Lionsguard.Slot.Item property.
		/// </summary>
		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(SlotItem), typeof(Slot), new PropertyMetadata(null, new PropertyChangedCallback(Slot.OnItemPropertyChanged)));
		/// <summary>
		/// Gets or sets the reference to the object that is set in the current slot.
		/// </summary>
		public SlotItem Item
		{
			get { return (SlotItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
			
		#endregion

		#region Events
		/// <summary>
		/// An event that is raised when the slot is clicked.
		/// </summary>
		public event RoutedEventHandler Click = delegate { };

		/// <summary>
		/// An event that is raised when the Use button is clicked.
		/// </summary>
		public event EventHandler Use = delegate { };

		/// <summary>
		/// An event that is raised when the Drop button is clicked.
		/// </summary>
		public event EventHandler Drop = delegate { };
		#endregion	

		/// <summary>
		/// Initializes a new instance of the Lionsguard.Slot class.
		/// </summary>
		public Slot()
		{
			this.DefaultStyleKey = typeof(Slot);
			this.MinWidth = 34;
			this.MinHeight = 34;
		}

		private static void OnSelectedPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Slot).GoToState(true);
		}

		private static void OnSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Slot).SetElementValues();
		}

		private static void OnBackgroundSourcePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Slot).SetElementValues();
		}
		private static void OnQuantityPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Slot).HandleQuantityChanged();
		}
		private static void OnIsDraggablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Slot).HandleIsDraggableChanged();
		}
		private static void OnIsDroppablePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Slot).HandleIsDraggableChanged();
		}
		private static void OnItemPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			(obj as Slot).HandleItemChanged();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			this.RootElement = base.GetTemplateChild("RootElement") as FrameworkElement;
			this.BackgroundElement = base.GetTemplateChild("BackgroundElement") as Image;
			this.IconElement = base.GetTemplateChild("IconElement") as Image;
			this.BorderElement = base.GetTemplateChild("BorderElement") as Border;
			this.MouseOverVisualElement = base.GetTemplateChild("MouseOverVisualElement") as Border;
			this.SelectedVisualElement = base.GetTemplateChild("SelectedVisualElement") as Border;
			this.QuantityBorderElement = base.GetTemplateChild("QuantityBorderElement") as Border;
			this.QuantityLabelElement = base.GetTemplateChild("QuantityLabelElement") as TextBlock;
			this.UseElement = base.GetTemplateChild("UseElement") as Path;
			this.DropElement = base.GetTemplateChild("DropElement") as TextBlock;

			if (this.RootElement != null)
			{
				this.RootElement.MouseEnter += new MouseEventHandler(OnMouseEnter);
				this.RootElement.MouseLeave += new MouseEventHandler(OnMouseLeave);
				this.RootElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseLeftButtonDown);
				this.RootElement.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseLeftButtonUp);
			}

			if (this.UseElement != null)
			{
				this.UseElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnUseElementMouseLeftButtonDown);
			}

			if (this.DropElement != null)
			{
				this.DropElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnDropElementMouseLeftButtonDown);
			}

			this.SetElementValues();

			this.GoToState(false);
		}

		private void OnDropElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.EnableDrop)
			{
				this.Drop(this, EventArgs.Empty);
				SlotManager.Instance.RaiseSlotDrop(this, EventArgs.Empty);
			}
		}

		private void OnUseElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.EnableUse)
			{
				this.Use(this, EventArgs.Empty);
				// Raise the SlotManager global slot use event.
				SlotManager.Instance.RaiseSlotUse(this, EventArgs.Empty);
			}
		}

		private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isPressed = false;
			this.GoToState(true);
		}

		private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_isPressed = true;
			this.GoToState(true);
			this.Click(this, new RoutedEventArgs());
			// Raise the SlotManager global slot click event.
			SlotManager.Instance.RaiseSlotClick(this, new RoutedEventArgs());
		}

		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			_isMouseOver = false;
			this.GoToState(true);
		}

		private void OnMouseEnter(object sender, MouseEventArgs e)
		{
			_isMouseOver = true;
			this.GoToState(true);
		}

		private void SetElementValues()
		{
			if (this.BackgroundElement != null)
			{
				this.BackgroundElement.Source = this.BackgroundSource;
			}
			if (this.IconElement != null)
			{
				this.IconElement.Source = this.Source;
			}
			if (this.QuantityLabelElement != null)
			{
				this.QuantityLabelElement.Text = this.Quantity.ToString();
			}
			if (this.UseElement != null)
			{
				if (this.Item != null && this.Item.IsUseable && this.EnableUse)
				{
					this.UseElement.Visibility = Visibility.Visible;
				}
				else
				{
					this.UseElement.Visibility = Visibility.Collapsed;
				}
			}
			if (this.DropElement != null)
			{
				if (this.Item != null && this.EnableDrop)
				{
					this.DropElement.Visibility = Visibility.Visible;
				}
				else
				{
					this.DropElement.Visibility = Visibility.Collapsed;
				}
			}
			this.HandleIsDraggableChanged();
			this.HandleQuantityChanged();
		}

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

		private void HandleItemChanged()
		{
			if (this.Item != null && this.Item.ImageSource != null)
			{
				this.Quantity = this.Item.Quantity;
				this.Source = this.Item.ImageSource;
				if (this.UseElement != null)
				{
					if (this.Item.IsUseable && this.EnableUse)
					{
						this.UseElement.Visibility = Visibility.Visible;
					}
					else
					{
						this.UseElement.Visibility = Visibility.Collapsed;
					}
				}
				if (this.DropElement != null)
				{
					if (this.EnableDrop)
					{
						this.DropElement.Visibility = Visibility.Visible;
					}
					else
					{
						this.DropElement.Visibility = Visibility.Collapsed;
					}
				}
			}
			else
			{
				if (this.UseElement != null)
				{
					this.UseElement.Visibility = Visibility.Collapsed;
				}
				if (this.DropElement != null)
				{
					this.DropElement.Visibility = Visibility.Collapsed;
				}
				this.Quantity = 0;
				this.Source = null;
			}
		}

		private void HandleQuantityChanged()
		{
			if (this.Quantity > 1)
			{
				_showQuantity = true;
			}
			else
			{
				_showQuantity = false;
			}
			this.GoToState(true);
		}

		private void HandleIsDraggableChanged()
		{
			// Drag and drop should only occur if the source image property and Item have been set.
			if (this.IsDraggable)
			{
				if (this.Item != null && this.Source != null && this.IconElement != null)
				{
					this.IconElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnIconElementMouseLeftButtonDown);
				}
			}
			else
			{
				if (this.Item != null && this.Source != null && this.IconElement != null)
				{
					this.IconElement.MouseLeftButtonDown -= new MouseButtonEventHandler(OnIconElementMouseLeftButtonDown);
				}
			}
		}

		#region Drag and Drop Methods
		private void OnIconElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsDraggable)
			{
				SlotManager.Instance.BeginDrag(this, e.GetPosition(null));
			}
		}
		#endregion
	}

	/// <summary>
	/// Represents an item that can be stored in a Lionsguard.Slot container.
	/// </summary>
	public class SlotItem : DependencyObject
	{
		/// <summary>
		/// Gets or sets the reference to the actual object stored in the slot.
		/// </summary>
		public object Item
		{
			get { return (object)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		/// <summary>
		/// Identifies the Lionsguard.SlotItem.Item property.
		/// </summary>
		public static readonly DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(object), typeof(SlotItem), new PropertyMetadata(null));
		
		/// <summary>
		/// Gets or sets the ImageSource of the image to display in the slot.
		/// </summary>
		public ImageSource ImageSource
		{
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		/// <summary>
		/// Identifies the Lionsguard.SlotItem.ImageSource property.
		/// </summary>
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(SlotItem), new PropertyMetadata(null));

		public int Quantity
		{
			get { return (int)GetValue(QuantityProperty); }
			set { SetValue(QuantityProperty, value); }
		}
		public static readonly DependencyProperty QuantityProperty = DependencyProperty.Register("Quantity", typeof(int), typeof(SlotItem), new PropertyMetadata(1));

		public bool IsUseable
		{
			get { return (bool)GetValue(IsUseableProperty); }
			set { SetValue(IsUseableProperty, value); }
		}
		public static readonly DependencyProperty IsUseableProperty = DependencyProperty.Register("IsUseable", typeof(bool), typeof(SlotItem), new PropertyMetadata(false));
			
		/// <summary>
		/// Initializes a new instance of the SlotItem class.
		/// </summary>
		public SlotItem()
			: this(null, null, 0, false)	
		{
		}

		/// <summary>
		/// Initializes a new instance of the SlotItem class and sets the item and imageUri values.
		/// </summary>
		/// <param name="item">The reference to the actual object stored in the slot.</param>
		/// <param name="imageSource">The Uri to the image to display in the slot.</param>
		public SlotItem(object item, ImageSource imageSource)
			: this(item, imageSource, 0, false)	
		{
		}

		/// <summary>
		/// Initializes a new instance of the SlotItem class and sets the item and imageUri values.
		/// </summary>
		/// <param name="item">The reference to the actual object stored in the slot.</param>
		/// <param name="imageSource">The Uri to the image to display in the slot.</param>
		/// <param name="quantity">The number of items stored in the slot. Will display if greater than 1.</param>
		public SlotItem(object item, ImageSource imageSource, int quantity)
			: this(item, imageSource, quantity, false)	
		{
		}

		/// <summary>
		/// Initializes a new instance of the SlotItem class and sets the item and imageUri values.
		/// </summary>
		/// <param name="item">The reference to the actual object stored in the slot.</param>
		/// <param name="imageSource">The Uri to the image to display in the slot.</param>
		/// <param name="quantity">The number of items stored in the slot. Will display if greater than 1.</param>
		/// <param name="isUseable">A value indicating whether or not the item specified can be used.</param>
		public SlotItem(object item, ImageSource imageSource, int quantity, bool isUseable)
		{
			this.Item = item;
			this.ImageSource = imageSource;
			this.Quantity = quantity;
			if (this.Quantity == 0) this.Quantity = 1;
			this.IsUseable = isUseable;
		}
	}

	/// <summary>
	/// Provides the managing class for handling drag and drop operations between slots.
	/// </summary>
	public class SlotManager
	{
		private Canvas Host { get; set; }
		private Image DragCursor { get; set; }
		private Point MousePosition { get; set; }
		private Slot SourceSlot { get; set; }
		private List<UIElement> SearchElements { get; set; }

		private bool HasProcessedEndDrag { get; set; }	

		private static object s_lock = new object();
		private static SlotManager s_instance = null;
		public static SlotManager Instance
		{
			get
			{
				if (s_instance == null)
				{
					lock (s_lock)
					{
						if (s_instance == null)
						{
							s_instance = new SlotManager();
						}
					}
				}
				return s_instance;
			}
		}

		private SlotManager()
		{
			this.SearchElements = new List<UIElement>();
		}

		/// <summary>
		/// An event that is raised when items change in source and destination slots as a result of drag and drop operation.
		/// </summary>
		public event SlotItemChangingEventHandler SlotItemChanging = delegate { };

		/// <summary>
		/// An event that is raised before a slot item change occurs to allow for outside handling of the slot item drop.
		/// </summary>
		public event SlotItemChangeEventHandler SlotItemChange = delegate { };

		/// <summary>
		/// An event that is raised when any Slot is clicked, use as a global event handler.
		/// </summary>
		public event RoutedEventHandler SlotClick = delegate { };

		/// <summary>
		/// An event that is raised when any Slot's Use command is executed, use as a global event handler.
		/// </summary>
		public event EventHandler SlotUse = delegate { };

		/// <summary>
		/// An event that is raised when any Slot's Drop command is executed, use as a global event handler.
		/// </summary>
		public event EventHandler SlotDrop = delegate { };

		/// <summary>
		/// Attaches the SlotManager to the specified host Canvas in order to allow drag and drop of Slot items.
		/// </summary>
		/// <param name="host">The hosting Canvas, must be canvas in order to provide drag and drop support.</param>
		public void Attach(Canvas host)
		{
			this.Host = host;
			this.Host.LostFocus += new RoutedEventHandler(Host_LostFocus);
		}

		private void Host_LostFocus(object sender, RoutedEventArgs e)
		{
			this.EndDrag(false);
		}

		/// <summary>
		/// Adds UIElement instances to search for Slots when performing the drop operation of a Slot.
		/// </summary>
		/// <param name="elements">An array of UIElement instances that contain Slots.</param>
		public void AddSearchElements(params UIElement[] elements)
		{
			this.SearchElements.AddRange(elements);
		}

		public void BeginDrag(Slot slot, Point mousePosition)
		{
			if (slot.Item != null && this.Host != null)
			{
				this.SourceSlot = slot;
				// Initialize the image that will replace the cursor, wire up events to the image.
				if (this.DragCursor == null)
				{
					this.DragCursor = new Image();

					this.DragCursor.LostFocus += new RoutedEventHandler(DragCursor_LostFocus);
					this.DragCursor.LostMouseCapture += new MouseEventHandler(DragCursor_LostMouseCapture);

					// Add the drag cursor to the host control and set the z-index very high.
					this.DragCursor.SetValue(Canvas.ZIndexProperty, 5000);
					this.Host.Children.Add(this.DragCursor);
				}
				// Hide the cursor and display the source image.
				this.Host.Cursor = Cursors.Hand;
				this.DragCursor.Source = slot.Item.ImageSource;
				
				this.MousePosition = mousePosition;
				this.DragCursor.SetValue(Canvas.LeftProperty, this.MousePosition.X);
				this.DragCursor.SetValue(Canvas.TopProperty, this.MousePosition.Y);
				this.DragCursor.Visibility = Visibility.Visible;

				this.HasProcessedEndDrag = false;

				this.DragCursor.CaptureMouse();

				this.DragCursor.MouseLeftButtonUp += new MouseButtonEventHandler(OnDragCursorMouseLeftButtonUp);
				this.DragCursor.MouseMove += new MouseEventHandler(OnDragCursorMouseMove);
			}
		}

		void DragCursor_LostMouseCapture(object sender, MouseEventArgs e)
		{
			this.EndDrag(false);
		}

		void DragCursor_LostFocus(object sender, RoutedEventArgs e)
		{
			this.EndDrag(false);
		}

		private void EndDrag(bool processDrop)
		{
			// This prevents the mouse up event or the end drag event firing more than once from causing the
			// events to bubble up and the drop be handled more than once.
			if (this.HasProcessedEndDrag) return;
			this.HasProcessedEndDrag = true;

			// Reshow the cursor and hide the source image.
			this.Host.Cursor = Cursors.Arrow;

			// Hide the drag cursor.
			if (this.DragCursor != null)
			{
				this.DragCursor.Visibility = Visibility.Collapsed;

				this.DragCursor.MouseLeftButtonUp -= new MouseButtonEventHandler(OnDragCursorMouseLeftButtonUp);
				this.DragCursor.MouseMove -= new MouseEventHandler(OnDragCursorMouseMove);
			}

			if (processDrop)
			{
				// Find a destination slot, using the search elements to find a Slot that intersects with the current
				// mouse position.
				Slot destSlot = null;
				UIElement destElement = null;

				// Foreach of the search elements attempt to find a slot at the x and y position.
				foreach (var element in this.SearchElements.Where(se => se.Visibility == Visibility.Visible))
				{
					var slot = (from v in VisualTreeHelper.FindElementsInHostCoordinates(this.MousePosition, element)
											  where (v as Slot) != null
											  select v as Slot).FirstOrDefault();
					if (slot != null)
					{
						destElement = element;
						destSlot = slot;
						break;
					}
				}

				if (destSlot != null)
				{
					// Just use the first available, unless it is the same as the source slot.
					if (destSlot != this.SourceSlot && this.SourceSlot.IsDraggable && destSlot.IsDroppable)
					{
						SlotItem destItem = destSlot.Item;
						SlotItem sourceItem = this.SourceSlot.Item;

						SlotItemChangeEventArgs args = new SlotItemChangeEventArgs(destElement, this.SourceSlot, destSlot, sourceItem, destItem);
						this.SlotItemChange(args);

						// Only continue this processing if the change was not handled.
						if (!args.Handled)
						{
							// Clear the source slot item.
							this.SourceSlot.Item = null;

							// If the destination slot is not empty then swap the two items.
							if (destItem != null)
							{
								SlotItemChangingEventArgs srcArgs = new SlotItemChangingEventArgs(this.SourceSlot, sourceItem, destItem);
								this.SlotItemChanging(srcArgs);
								if (!srcArgs.Cancel)
								{
									this.SourceSlot.Item = destItem;
								}
							}
							SlotItemChangingEventArgs destArgs = new SlotItemChangingEventArgs(destSlot, destItem, sourceItem);
							this.SlotItemChanging(destArgs);
							if (!destArgs.Cancel)
							{
								destSlot.Item = sourceItem;
							}
						}
					}
				}
			}
		}

		private void OnDragCursorMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			((UIElement)sender).ReleaseMouseCapture();

			this.MousePosition = e.GetPosition(null);

			this.EndDrag(true);
		}
		private void OnDragCursorMouseMove(object sender, MouseEventArgs e)
		{
			Point position = e.GetPosition(null);

			double deltaX = position.X - this.MousePosition.X;
			double deltaY = position.Y - this.MousePosition.Y;

			Point newPosition = new Point(
				((double)this.DragCursor.GetValue(Canvas.LeftProperty)) + deltaX,
				((double)this.DragCursor.GetValue(Canvas.TopProperty)) + deltaY);

			this.DragCursor.SetValue(Canvas.LeftProperty, newPosition.X);
			this.DragCursor.SetValue(Canvas.TopProperty, newPosition.Y);

			this.MousePosition = position;
		}

		internal void RaiseSlotClick(object sender, RoutedEventArgs e)
		{
			this.SlotClick(sender, e);
		}

		internal void RaiseSlotUse(object sender, EventArgs e)
		{
			this.SlotUse(sender, e);
		}

		internal void RaiseSlotDrop(object sender, EventArgs e)
		{
			this.SlotDrop(sender, e);
		}
	}

	public delegate void SlotItemChangingEventHandler(SlotItemChangingEventArgs e);
	public class SlotItemChangingEventArgs : EventArgs
	{
		public Slot Slot { get; set; }
		public SlotItem OldValue { get; set; }
		public SlotItem NewValue { get; set; }
		public bool Cancel { get; set; }	

		public SlotItemChangingEventArgs(Slot slot, SlotItem oldValue, SlotItem newValue)
		{
			this.Slot = slot;
			this.OldValue = oldValue;
			this.NewValue = NewValue;
		}
	}

	public delegate void SlotItemChangeEventHandler(SlotItemChangeEventArgs e);
	public class SlotItemChangeEventArgs : EventArgs
	{
		public UIElement DestinationElement { get; set; }
		public Slot SourceSlot { get; set; }
		public Slot DestinationSlot { get; set; }	
		public SlotItem SourceValue { get; set; }
		public SlotItem DestinationValue { get; set; }
		public bool Handled { get; set; }

		public SlotItemChangeEventArgs(UIElement destinationElement, Slot sourceSlot, Slot destinationSlot, SlotItem sourceValue, SlotItem destinationValue)
		{
			this.DestinationElement = destinationElement;
			this.SourceSlot = sourceSlot;
			this.DestinationSlot = destinationSlot;
			this.SourceValue = sourceValue;
			this.DestinationValue = destinationValue;
		}
	}
}

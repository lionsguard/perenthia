using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Perenthia
{
	/// <summary>
	/// Represents currency within the Perenthia Game.
	/// </summary>
	public class Currency
	{
		private const int High = 10000;
		private const int Low = 100;

		private int _value = 0;
		private int _gold = 0;
		private int _silver = 0;
		private int _copper = 0;

		public event CurrencyValueChangedEventHandler Changed = delegate { };

		/// <summary>
		/// Gets or sets the Double value of the currency.
		/// </summary>
		public int Value
		{
			get { return _value; }
			set
			{
				_value = value;
				this.ParseCurrency();
				this.Changed(new CurrencyValueChangedEventArgs(_value));
			}
		}

		/// <summary>
		/// Gets the Gold component of the currency.
		/// </summary>
		public int Gold
		{
			get { return _gold; }
			set { this.Value += (value * High); }
		}

		/// <summary>
		/// Gets the Silver component of the currency.
		/// </summary>
		public int Silver
		{
			get { return _silver; }
			set { this.Value += (value * Low); }
		}

		/// <summary>
		/// Gets the Copper component of the currency.
		/// </summary>
		public int Copper
		{
			get { return _copper; }
			set { this.Value += value; }
		}

		/// <summary>
		/// Initializes a new instance of the Currency class.
		/// </summary>
		public Currency()
		{
		}

		/// <summary>
		/// Initializes a new instance of the Currency class and presets the Value component.
		/// </summary>
		/// <param name="value">The currency value represented as a whole number.</param>
		public Currency(int value)
		{
			this.Value = value;
		}

		private void ParseCurrency()
		{
			if (_value != 0)
			{
				double val = (double)_value;
				_gold = (int)Math.Floor(val / (double)High);
				_silver = (int)Math.Floor(val % (double)High / (double)Low);
				_copper = (int)Math.Floor(val % (double)Low);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (this.Gold > 0)
			{
				sb.Append(this.Gold).Append(" gold");
			}
			if (this.Silver > 0)
			{
				if (sb.Length > 0) sb.Append(", ");
				sb.Append(this.Silver).Append(" silver");
			}
			if (this.Copper > 0)
			{
				if (sb.Length > 0) sb.Append(", ");
				sb.Append(this.Copper).Append(" copper");
			}
			return sb.ToString();
		}
	}

	public delegate void CurrencyValueChangedEventHandler(CurrencyValueChangedEventArgs e);

	public class CurrencyValueChangedEventArgs : EventArgs
	{
		public int Value { get; set; }

		public CurrencyValueChangedEventArgs(int value)
		{
			this.Value = value;
		}
	}
}

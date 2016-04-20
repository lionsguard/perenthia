using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Commerce
{
	[Serializable]
	public class Product
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public decimal UnitPrice { get; set; }
		public int PurchaseQuantity { get; set; }
		public string Description { get; set; }
		public string ImageUrl { get; set; }
		public int Quantity { get; set; }
		public bool IsActive { get; set; }

		public decimal Total
		{
			get { return (decimal)(this.UnitPrice * this.Quantity); }
		}

		public string FullName
		{
			get { return String.Format("{0} - <strong>{1}</strong>", this.Name, this.UnitPrice.ToString("C")); }
		}

		public Product() { }

		internal Product(SqlNullDataReader reader)
		{
			this.Quantity = 1;
			this.ID = reader.GetInt32("ProductId");
			this.Name = reader.GetString("ProductName");
			this.UnitPrice = reader.GetDecimal("UnitPrice");
			this.PurchaseQuantity = reader.GetInt32("PurchaseQuantity");
			this.Description = reader.GetString("Description");
			this.ImageUrl = reader.GetString("ImageUrl");
			this.IsActive = reader.GetBoolean("IsActive");
		}
	}
}

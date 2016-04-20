using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Commerce
{
	public class Order
	{
		public long ID { get; set; }
		public int UserID { get; set; }
		public double Total { get; set; }
		public string PayPalPayerID { get; set; }	
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Street1 { get; set; }
		public string Street2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip { get; set; }
		public string Phone { get; set; }
		public DateTime DateCreated { get; set; }	
		public List<Product> Products { get; set; }

		public Order()
		{
			this.DateCreated = DateTime.Now;
			this.Products = new List<Product>();
		}

		internal Order(SqlNullDataReader reader)
			: this()
		{
			this.ID = reader.GetInt64("OrderId");
			this.UserID = reader.GetInt32("UserId");
			this.PayPalPayerID = reader.GetString("PayPalPayerID");
			this.Total = reader.GetDouble("Total");
			this.DateCreated = reader.GetDateTime("DateCreated");
			this.FirstName = reader.GetString("FirstName");
			this.LastName = reader.GetString("LastName");
			this.Street1 = reader.GetString("Street1");
			this.Street2 = reader.GetString("Street2");
			this.City = reader.GetString("City");
			this.State = reader.GetString("State");
			this.Phone = reader.GetString("Phone");
		}

		public void Save()
		{
			CommerceManager.CreateOrder(this);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Lionsguard.Data;

namespace Lionsguard.Commerce
{
	public class Transaction
	{
		public long ID { get; set; }
		public string IPAddress { get; set; }
		public double Total { get; set; }
		public string CreditCardLast4 { get; set; }
		public DateTime DateCreated { get; set; }	
		public Order Order { get; set; }

		public Transaction()
		{
			this.DateCreated = DateTime.Now;
		}

		internal Transaction(SqlNullDataReader reader)
			: this()
		{
			this.ID = reader.GetInt64("TransactionId");
			this.IPAddress = reader.GetString("IPAddress");
			this.Total = reader.GetDouble("Total");
			this.CreditCardLast4 = reader.GetString("CreditCardLast4");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Lionsguard.Data;
using Lionsguard.Commerce;
using Lionsguard.Providers;

namespace Lionsguard.Providers
{
	public class SqlCommerceProvider : CommerceProvider
	{
		private string _connectionString;

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			base.Initialize(name, config);

			_connectionString = ProviderUtil.GetConnectionString(config);

			ProviderUtil.CheckUnrecognizedAttributes(config);
		}

		public override List<Product> GetProducts()
		{
			List<Product> list = new List<Product>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Products_GetProducts"))
			{
				while (reader.Read())
				{
					list.Add(new Product(reader));
				}
			}
			return list;
		}

		public override List<Product> GetActiveProducts()
		{
			List<Product> list = new List<Product>();
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Products_GetActiveProducts"))
			{
				while (reader.Read())
				{
					list.Add(new Product(reader));
				}
			}
			return list;
		}

		public override Product GetProduct(int id)
		{
			using (SqlNullDataReader reader = SqlHelper.ExecuteReader(_connectionString, "dbo.lg_Products_GetProduct",
				SqlHelper.CreateInputParam("@ProductId", SqlDbType.Int, id)))
			{
				if (reader.Read())
				{
					return new Product(reader);
				}
			}
			return null;
		}

		public override void CreateOrder(Order order)
		{
			//@UserId				int, 
			//@PayPalPayerID		nvarchar(256), 
			//@Total				money, 
			//@FirstName			nvarchar(32), 
			//@LastName			nvarchar(32), 
			//@Street1			nvarchar(80), 
			//@Street2			nvarchar(80), 
			//@City				nvarchar(40), 
			//@State				nvarchar(3), 
			//@Zip				nvarchar(15), 
			//@Phone				nvarchar(15), 
			//@DateCreated		datetime,
			//@OrderId			bigint output
			using (SqlCommand cmd = SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Products_CreateOrder",
				SqlHelper.CreateInputParam("@UserId", SqlDbType.Int, order.UserID),
				SqlHelper.CreateInputParam("@PayPalPayerID", SqlDbType.NVarChar, order.PayPalPayerID),
				SqlHelper.CreateInputParam("@Total", SqlDbType.Money, order.Total),
				SqlHelper.CreateInputParam("@FirstName", SqlDbType.NVarChar, order.FirstName),
				SqlHelper.CreateInputParam("@LastName", SqlDbType.NVarChar, order.LastName),
				SqlHelper.CreateInputParam("@Street1", SqlDbType.NVarChar, order.Street1),
				SqlHelper.CreateInputParam("@Street2", SqlDbType.NVarChar, order.Street2),
				SqlHelper.CreateInputParam("@City", SqlDbType.NVarChar, order.City),
				SqlHelper.CreateInputParam("@State", SqlDbType.NVarChar, order.State),
				SqlHelper.CreateInputParam("@Zip", SqlDbType.NVarChar, order.Zip),
				SqlHelper.CreateInputParam("@Phone", SqlDbType.NVarChar, order.Phone),
				SqlHelper.CreateInputParam("@DateCreated", SqlDbType.DateTime, order.DateCreated),
				SqlHelper.CreateOutputParam("@OrderId", SqlDbType.BigInt)))
			{
				order.ID = Convert.ToInt64(cmd.Parameters["@OrderId"].Value);
				foreach (var prod in order.Products)
				{
					//@OrderId			bigint,
					//@ProductId			int,
					//@Quantity			int,
					//@Total				money
					SqlHelper.ExecuteNonQuery(_connectionString, "dbo.lg_Products_AddProductToOrder",
						SqlHelper.CreateInputParam("@OrderId", SqlDbType.BigInt, order.ID),
						SqlHelper.CreateInputParam("@ProductId", SqlDbType.Int, prod.ID),
						SqlHelper.CreateInputParam("@Quantity", SqlDbType.Int, prod.Quantity),
						SqlHelper.CreateInputParam("@Total", SqlDbType.Money, prod.Total));
				}
			}
		}
	}
}

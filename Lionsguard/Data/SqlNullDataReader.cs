using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

namespace Lionsguard.Data
{
	/// <summary>
	/// Represents a SqlDataReader that will return a default value for the various Get methods when the actual value is DBNull and 
	/// allow column access via the column name or ordinal position.
	/// </summary>
	public class SqlNullDataReader : NullDataReader
	{
		/// <summary>
		/// Initializes a new instance of the SqlNullDataReader class.
		/// </summary>
		/// <param name="reader"></param>
		public SqlNullDataReader(IDataReader reader)
			: base(reader)
		{
		}

		/// <summary>
		/// Gets the value of the specified column as an XML value.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>A System.Data.SqlTypes.SqlXml value that contains the XML stored within the corresponding field.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to System.Data.DataTableReader.FieldCount - 1</exception>
		/// <exception cref="System.InvalidCastException">The retrieved data is not compatible with the System.Data.SqlTypes.SqlXml type.</exception>
		/// <exception cref="System.InvalidOperationException">An attempt was made to read or access columns in a closed System.Data.SqlClient.SqlDataReader.</exception>
		public SqlXml GetSqlXml(int i)
		{
			if (!this.IsDBNull(i))
			{
				if (InnerReader is SqlDataReader)
				{
					return (InnerReader as SqlDataReader).GetSqlXml(i);
				}
			}
			return SqlXml.Null;
		}

		/// <summary>
		/// Gets the value of the specified column as an XML value.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>A System.Data.SqlTypes.SqlXml value that contains the XML stored within the corresponding field.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">The index passed was outside the range of 0 to System.Data.DataTableReader.FieldCount - 1</exception>
		/// <exception cref="System.InvalidCastException">The retrieved data is not compatible with the System.Data.SqlTypes.SqlXml type.</exception>
		/// <exception cref="System.InvalidOperationException">An attempt was made to read or access columns in a closed System.Data.SqlClient.SqlDataReader.</exception>
		public SqlXml GetSqlXml(string name)
		{
			return this.GetSqlXml(this.GetOrdinal(name));
		}
	}
}

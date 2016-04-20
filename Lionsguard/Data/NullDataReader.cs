using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Lionsguard.Data
{
	/// <summary>
	/// Provides the abstract base class for deriving IDataReader instances that will return default values for DBNull columns and 
	/// allow column access via the column name or ordinal position.
	/// </summary>
	public abstract class NullDataReader : IDataReader, IDataRecord, IDisposable
	{
		/// <summary>
		/// The underlying IDataReader instance.
		/// </summary>
		protected IDataReader InnerReader;

		/// <summary>
		/// Initializes a new instance of the NullDataReader class.
		/// </summary>
		/// <param name="reader">The underlying IDataReader instance.</param>
		protected NullDataReader(IDataReader reader)
		{
			InnerReader = reader;
		}

		#region IDataReader Members

		/// <summary>
		/// Closes the System.Data.IDataReader 0bject.
		/// </summary>
		public void Close()
		{
			InnerReader.Close();
		}

		/// <summary>
		/// Gets a value indicating the depth of nesting for the current row.
		/// </summary>
		/// <value>The level of nesting.</value>
		public int Depth
		{
			get { return InnerReader.Depth; }
		}

		/// <summary>
		/// Returns a System.Data.DataTable that describes the column metadata of the System.Data.IDataReader.
		/// </summary>
		/// <returns>A System.Data.DataTable that describes the column metadata.</returns>
		/// <exception cref="System.InvalidOperationException">The System.Data.IDataReader is closed.</exception>
		public DataTable GetSchemaTable()
		{
			return InnerReader.GetSchemaTable();
		}

		/// <summary>
		/// Gets a value indicating whether the data reader is closed.
		/// </summary>
		/// <value>true if the data reader is closed; otherwise, false.</value>
		public bool IsClosed
		{
			get { return InnerReader.IsClosed; }
		}

		/// <summary>
		/// Advances the data reader to the next result, when reading the results of batch SQL statements.
		/// </summary>
		/// <returns>true if there are more rows; otherwise, false.</returns>
		public bool NextResult()
		{
			return InnerReader.NextResult();
		}

		/// <summary>
		/// Advances the System.Data.IDataReader to the next record.
		/// </summary>
		/// <returns>true if there are more rows; otherwise, false.</returns>
		public bool Read()
		{
			return InnerReader.Read();
		}

		/// <summary>
		/// Gets the number of rows changed, inserted, or deleted by execution of the SQL statement.
		/// </summary>
		/// <value>The number of rows changed, inserted, or deleted; 0 if no rows were affected or 
		/// the statement failed; and -1 for SELECT statements.</value>
		public int RecordsAffected
		{
			get { return InnerReader.RecordsAffected; }
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources
		/// </summary>
		public void Dispose()
		{
			InnerReader.Dispose();
		}

		#endregion

		#region IDataRecord Members

		/// <summary>
		/// Gets the number of columns in the current row.
		/// </summary>
		/// <value>When not positioned in a valid recordset, 0; otherwise the number of columns in the 
		/// current record. The default is -1.</value>
		public int FieldCount
		{
			get { return InnerReader.FieldCount; }
		}

		/// <summary>
		/// Gets the value of the specified column as a Boolean.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The value of the column.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public bool GetBoolean(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetBoolean(i);
			}
			return false;
		}
		/// <summary>
		/// Gets the value of the specified column as a Boolean.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The value of the column.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public bool GetBoolean(string name)
		{
			return this.GetBoolean(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the 8-bit unsigned integer value of the specified column.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The 8-bit unsigned integer value of the specified column.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public byte GetByte(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetByte(i);
			}
			return 0;
		}
		/// <summary>
		/// Gets the 8-bit unsigned integer value of the specified column.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The 8-bit unsigned integer value of the specified column.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public byte GetByte(string name)
		{
			return this.GetByte(this.GetOrdinal(name));
		}

		/// <summary>
		/// Reads a stream of bytes from the specified column offset into the buffer as an array, starting at the given buffer offset.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <param name="fieldOffset">The index within the field from which to start the read operation.</param>
		/// <param name="buffer">The buffer into which to read the stream of bytes.</param>
		/// <param name="bufferoffset">The index for buffer to start the read operation.</param>
		/// <param name="length">The number of bytes to read.</param>
		/// <returns>The actual number of bytes read.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{
			return InnerReader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
		}

		/// <summary>
		/// Gets the character value of the specified column.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <returns>The character value of the specified column.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public char GetChar(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetChar(i);
			}
			return new char();
		}
		/// <summary>
		/// Gets the character value of the specified column.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The character value of the specified column.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public char GetChar(string name)
		{
			return this.GetChar(this.GetOrdinal(name));
		}

		/// <summary>
		/// Reads a stream of characters from the specified column offset into the buffer as an array, starting at the given buffer offset.
		/// </summary>
		/// <param name="i">The zero-based column ordinal.</param>
		/// <param name="fieldoffset">The index within the row from which to start the read operation.</param>
		/// <param name="buffer">The buffer into which to read the stream of bytes.</param>
		/// <param name="bufferoffset">The index for buffer to start the read operation.</param>
		/// <param name="length">The number of bytes to read.</param>
		/// <returns>The actual number of characters read.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			return InnerReader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
		}

		/// <summary>
		/// Gets an System.Data.IDataReader to be used when the field points to more remote structured data.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>An System.Data.IDataReader to be used when the field points to more remote structured data.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public IDataReader GetData(int i)
		{
			return InnerReader.GetData(i);
		}

		/// <summary>
		/// Gets the data type information for the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The data type information for the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public string GetDataTypeName(int i)
		{
			return InnerReader.GetDataTypeName(i);
		}

		/// <summary>
		/// Gets the date and time data value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The date and time data value of the spcified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public DateTime GetDateTime(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetDateTime(i);
			}
			return DateTime.MinValue;
		}
		/// <summary>
		/// Gets the date and time data value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The date and time data value of the spcified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public DateTime GetDateTime(string name)
		{
			return this.GetDateTime(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the fixed-position numeric value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The fixed-position numeric value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public decimal GetDecimal(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetDecimal(i);
			}
			return 0;
		}
		/// <summary>
		/// Gets the fixed-position numeric value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The fixed-position numeric value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public decimal GetDecimal(string name)
		{
			return this.GetDecimal(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the double-precision floating point number of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The double-precision floating point number of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public double GetDouble(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetDouble(i);
			}
			return 0;
		}
		/// <summary>
		/// Gets the double-precision floating point number of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The double-precision floating point number of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public double GetDouble(string name)
		{
			return this.GetDouble(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the System.Type information corresponding to the type of System.Object that would be 
		/// returned from System.Data.IDataRecord.GetValue(System.Int32).
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The System.Type information corresponding to the type of System.Object that would 
		/// be returned from System.Data.IDataRecord.GetValue(System.Int32).</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public Type GetFieldType(int i)
		{
			return InnerReader.GetFieldType(i);
		}

		/// <summary>
		/// Gets the single-precision floating point number of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The single-precision floating point number of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public float GetFloat(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetFloat(i);
			}
			return 0;
		}
		/// <summary>
		/// Gets the single-precision floating point number of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The single-precision floating point number of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public float GetFloat(string name)
		{
			return this.GetFloat(this.GetOrdinal(name));
		}

		/// <summary>
		/// Returns the GUID value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The GUID value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public Guid GetGuid(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetGuid(i);
			}
			return Guid.Empty;
		}
		/// <summary>
		/// Returns the GUID value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The GUID value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public Guid GetGuid(string name)
		{
			return this.GetGuid(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the 16-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The 16-bit signed integer value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public short GetInt16(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetInt16(i);
			}
			return 0;
		}
		/// <summary>
		/// Gets the 16-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The 16-bit signed integer value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public short GetInt16(string name)
		{
			return this.GetInt16(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the 32-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The 32-bit signed integer value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public int GetInt32(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetInt32(i);
			}
			return 0;
		}
		/// <summary>
		/// Gets the 32-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The 32-bit signed integer value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public int GetInt32(string name)
		{
			return this.GetInt32(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the 64-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The 64-bit signed integer value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public long GetInt64(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetInt64(i);
			}
			return 0;
		}
		/// <summary>
		/// Gets the 64-bit signed integer value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The 64-bit signed integer value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public long GetInt64(string name)
		{
			return this.GetInt64(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the name for the field to find.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The name of the field or the empty string (""), if there is no value to return.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public string GetName(int i)
		{
			return InnerReader.GetName(i);
		}

		/// <summary>
		/// Return the index of the named field.
		/// </summary>
		/// <param name="name">The name of the field to find.</param>
		/// <returns>The index of the named field.</returns>
		public int GetOrdinal(string name)
		{
			return InnerReader.GetOrdinal(name);
		}

		/// <summary>
		/// Gets the string value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The string value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public string GetString(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetString(i);
			}
			return (string)null;
		}
		/// <summary>
		/// Gets the string value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The string value of the specified field.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public string GetString(string name)
		{
			return this.GetString(this.GetOrdinal(name));
		}

		/// <summary>
		/// Return the value of the specified field.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>The System.Object which will contain the field value upon return.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public object GetValue(int i)
		{
			if (!this.IsDBNull(i))
			{
				return InnerReader.GetValue(i);
			}
			return null;
		}
		/// <summary>
		/// Return the value of the specified field.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The System.Object which will contain the field value upon return.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public object GetValue(string name)
		{
			return this.GetValue(this.GetOrdinal(name));
		}

		/// <summary>
		/// Gets all the attribute fields in the collection for the current record.
		/// </summary>
		/// <param name="values">An array of System.Object to copy the attribute fields into.</param>
		/// <returns>The number of instances of System.Object in the array.</returns>
		public int GetValues(object[] values)
		{
			return InnerReader.GetValues(values);
		}

		/// <summary>
		/// Return whether the specified field is set to null.
		/// </summary>
		/// <param name="i">The index of the field to find.</param>
		/// <returns>true if the specified field is set to null. Otherwise, false.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public bool IsDBNull(int i)
		{
			return InnerReader.IsDBNull(i);
		}
		/// <summary>
		/// Return whether the specified field is set to null.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>true if the specified field is set to null. Otherwise, false.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public bool IsDBNull(string name)
		{
			return InnerReader.IsDBNull(InnerReader.GetOrdinal(name));
		}

		/// <summary>
		/// Gets the column with the specified name.
		/// </summary>
		/// <param name="name">The name of the column to find.</param>
		/// <returns>The column with the specified name as an System.Object.</returns>
		/// <exception cref="System.IndexOutOfRangeException">No column with the specified name was found.</exception>
		public object this[string name]
		{
			get { return InnerReader[name]; }
		}

		/// <summary>
		/// Gets the column located at the specified index.
		/// </summary>
		/// <param name="i">The index of the column to get.</param>
		/// <returns>The column located at the specified index as an System.Object.</returns>
		/// <exception cref="System.IndexOutOfRangeException">The index passed was outside the range of 0 through System.Data.IDataRecord.FieldCount.</exception>
		public object this[int i]
		{
			get { return InnerReader[i]; }
		}

		#endregion
	}
}

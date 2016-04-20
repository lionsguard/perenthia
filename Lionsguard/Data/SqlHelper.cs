using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.Caching;

namespace Lionsguard.Data
{
	/// <summary>
	/// A static helper class for executing SQL statements and procedures against an MS SQL database.
	/// </summary>
    public static class SqlHelper
    {
		#region Parameter Methods

		/// <summary>
		/// Creates a SqlParameter class set as an input param for a stored procedure.
		/// </summary>
		/// <param name="paramName">The name of the parameter.</param>
		/// <param name="dbType">The SqlDbType of the parameter to create.</param>
		/// <param name="objValue">The value to assign to the parameter.</param>
		/// <returns>A new SqlParameter instance.</returns>
		public static SqlParameter CreateInputParam(string paramName, SqlDbType dbType, object objValue)
        {
            SqlParameter param = new SqlParameter(paramName, dbType);
            if (objValue == null)
            {
                param.IsNullable = true;
                param.Value = DBNull.Value;
                return param;
            }
            param.Value = objValue;
            return param;
        }

		/// <summary>
		/// Creates a SqlParameter class set as an output param for a stored procedure.
		/// </summary>
		/// <param name="paramName">The name of the parameter.</param>
		/// <param name="dbType">The SqlDbType of the parameter to create.</param>
		/// <returns>A new SqlParameter instance.</returns>
        public static SqlParameter CreateOutputParam(string paramName, SqlDbType dbType)
        {
            SqlParameter param = SqlHelper.CreateInputParam(paramName, dbType, null);
            param.Direction = ParameterDirection.Output;
            return param;
        }

		/// <summary>
		/// Creates a SqlParameter class set as an input/output param for a stored procedure.
		/// </summary>
		/// <param name="paramName">The name of the parameter.</param>
		/// <param name="dbType">The SqlDbType of the parameter to create.</param>
		/// <param name="objValue">The value to assign to the parameter.</param>
		/// <returns>A new SqlParameter instance.</returns>
        public static SqlParameter CreateInputOutputParam(string paramName, SqlDbType dbType, object objValue)
        {
            SqlParameter param = SqlHelper.CreateInputParam(paramName, dbType, objValue);
            param.Direction = ParameterDirection.InputOutput;
            return param;
        }

		/// <summary>
		/// Creates a SqlParameter class set as a return param for a stored procedure.
		/// </summary>
		/// <param name="paramName">The name of the parameter.</param>
		/// <returns>A new SqlParameter instance.</returns>
        public static SqlParameter CreateReturnParam(string paramName)
        {
            SqlParameter param = new SqlParameter(paramName, SqlDbType.Int);
            param.Direction = ParameterDirection.ReturnValue;
            return param;
        }

        #endregion

        #region Data Methods

		#region ExecuteReader
		/// <summary>
		/// Creates a database connection and opens and returns SqlNullDataReader instance. This method will attempt 
		/// to execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <returns>An instance of SqlNullDataReader.</returns>
		public static SqlNullDataReader ExecuteReader(string connectionString, string query)
        {
			return ExecuteReader(connectionString, query, (string)null, CommandType.StoredProcedure, null);
        }
		/// <summary>
		/// Creates a database connection and opens and returns SqlNullDataReader instance.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <returns>An instance of SqlNullDataReader.</returns>
		public static SqlNullDataReader ExecuteReader(string connectionString, string query, CommandType commandType)
		{
			return ExecuteReader(connectionString, query, (string)null, commandType, null);
		}
		/// <summary>
		/// Creates a database connection and opens and returns SqlNullDataReader instance. This method will attempt 
		/// to execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>An instance of SqlNullDataReader.</returns>
		public static SqlNullDataReader ExecuteReader(string connectionString, string query, params SqlParameter[] parameters)
		{
			return ExecuteReader(connectionString, query, (string)null, CommandType.StoredProcedure, parameters);
		}
		/// <summary>
		/// Creates a database connection and opens and returns SqlNullDataReader instance.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>An instance of SqlNullDataReader.</returns>
		public static SqlNullDataReader ExecuteReader(string connectionString, string query, CommandType commandType, params SqlParameter[] parameters)
		{
			return ExecuteReader(connectionString, query, (string)null, commandType, parameters);
		}
		/// <summary>
		/// Creates a database connection and opens and returns SqlNullDataReader instance. This method will attempt 
		/// to execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="cacheKey">The key used with SqlCacheDependency. ** NOTE: Not Implemented.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>An instance of SqlNullDataReader.</returns>
		public static SqlNullDataReader ExecuteReader(string connectionString, string query, string cacheKey, params SqlParameter[] parameters)
		{
			return ExecuteReader(connectionString, query, (string)null, CommandType.StoredProcedure, parameters);
        }
		/// <summary>
		/// Creates a database connection and opens and returns SqlNullDataReader instance.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="cacheKey">The key used with SqlCacheDependency. ** NOTE: Not Implemented.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>An instance of SqlNullDataReader.</returns>
		public static SqlNullDataReader ExecuteReader(
			string connectionString, string query, string cacheKey, CommandType commandType, params SqlParameter[] parameters)
		{
			return ExecuteReader(connectionString, query, cacheKey, commandType, 20, parameters);
		}
		/// <summary>
		/// Creates a database connection and opens and returns SqlNullDataReader instance.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="cacheKey">The key used with SqlCacheDependency. ** NOTE: Not Implemented.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="commandTimeout">The timeout value for this command.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>An instance of SqlNullDataReader.</returns>
		public static SqlNullDataReader ExecuteReader(
			string connectionString, string query, string cacheKey, CommandType commandType, int commandTimeout, params SqlParameter[] parameters)
		{
			SqlConnection conn = new SqlConnection(connectionString);
			using (SqlCommand cmd = new SqlCommand(query, conn))
			{
				cmd.CommandType = commandType;
				cmd.CommandTimeout = commandTimeout;
				if (parameters != null && parameters.Length > 0)
				{
					cmd.Parameters.AddRange(parameters);
				}
				conn.Open();
				// SqCaching
				return new SqlNullDataReader(cmd.ExecuteReader(CommandBehavior.CloseConnection));
			}
		}
		/// <summary>
		/// Creates a database connection and opens and returns SqlNullDataReader instance.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="command">The SqlCommand object to use when executing the query.</param>
		public static SqlNullDataReader ExecuteReader(string connectionString, SqlCommand command)
		{
			SqlConnection conn = new SqlConnection(connectionString);
			command.Connection = conn;
			conn.Open();
			return new SqlNullDataReader(command.ExecuteReader(CommandBehavior.CloseConnection));
		}
		#endregion

		#region ExecuteScalar
		/// <summary>
		/// Creates a database connection and executes the specified query, returning the first column of the first row of the results. 
		/// This method will execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <returns>The value of the first colunm of the first row of the results.</returns>
		public static object ExecuteScalar(string connectionString, string query)
        {
            return ExecuteScalar(connectionString, query, CommandType.StoredProcedure, null);
        }
		/// <summary>
		/// Creates a database connection and executes the specified query, returning the first column of the first row of the results.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <returns>The value of the first colunm of the first row of the results.</returns>
		public static object ExecuteScalar(string connectionString, string query, CommandType commandType)
		{
			return ExecuteScalar(connectionString, query, commandType, null);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query, returning the first column of the first row of the results. 
		/// This method will execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>The value of the first colunm of the first row of the results.</returns>
		public static object ExecuteScalar(string connectionString, string query, params SqlParameter[] parameters)
		{
			return ExecuteScalar(connectionString, query, CommandType.StoredProcedure, parameters);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query, returning the first column of the first row of the results.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>The value of the first colunm of the first row of the results.</returns>
		public static object ExecuteScalar(string connectionString, string query, CommandType commandType, params SqlParameter[] parameters)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.CommandType = commandType;
					if (parameters != null && parameters.Length > 0)
					{
						cmd.Parameters.AddRange(parameters);
					}
					conn.Open();
					return cmd.ExecuteScalar();
				}
			}
		}
		/// <summary>
		/// Creates a database connection and executes the specified query, returning the first column of the first row of the results.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="command">The SqlCommand to execute.</param>
		/// <returns>The value of the first colunm of the first row of the results.</returns>
		public static object ExecuteScalar(string connectionString, SqlCommand command)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				command.Connection = conn;
				conn.Open();
				return command.ExecuteScalar();
			}
		}
		#endregion

		#region ExecuteNonQuery
		/// <summary>
		/// Creates a database connection and executes the specified query. This method will execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, string query)
        {
            return ExecuteNonQuery(connectionString, query, CommandType.StoredProcedure, null);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query. This method will execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="useTransaction">Causes the statement to execute within a transaction.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, string query, bool useTransaction)
		{
			return ExecuteNonQuery(connectionString, query, CommandType.StoredProcedure, useTransaction, null);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, string query, CommandType commandType)
		{
			return ExecuteNonQuery(connectionString, query, commandType, null);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="useTransaction">Causes the statement to execute within a transaction.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, string query, CommandType commandType, bool useTransaction)
		{
			return ExecuteNonQuery(connectionString, query, commandType, useTransaction, null);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query. This method will execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, string query, params SqlParameter[] parameters)
		{
			return ExecuteNonQuery(connectionString, query, CommandType.StoredProcedure, parameters);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query. This method will execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <param name="useTransaction">Causes the statement to execute within a transaction.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, string query, bool useTransaction, params SqlParameter[] parameters)
		{
			return ExecuteNonQuery(connectionString, query, CommandType.StoredProcedure, useTransaction, parameters);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, string query, CommandType commandType, params SqlParameter[] parameters)
		{
			return ExecuteNonQuery(connectionString, query, commandType, false, parameters);
		}
		/// <summary>
		/// Creates a database connection and executes the specified query.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <param name="useTransaction">Causes the statement to execute within a transaction.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, string query, CommandType commandType, bool useTransaction, params SqlParameter[] parameters)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				SqlCommand cmd = new SqlCommand(query, conn);
				cmd.CommandType = commandType;
				if (parameters != null && parameters.Length > 0)
				{
					cmd.Parameters.AddRange(parameters);
				}

				conn.Open();

				SqlTransaction trans = null;
				try
				{
					if (useTransaction)
					{
						trans = conn.BeginTransaction();
					}

					cmd.ExecuteNonQuery();

					if (useTransaction) trans.Commit();
				}
				catch (Exception ex)
				{
					if (trans != null) trans.Rollback();
					throw ex;
				}
				finally
				{
					if (trans != null) trans.Dispose();
				}
				return cmd;
			}
		}
		/// <summary>
		/// Creates a database connection and executes the specified query.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="command">The SqlCommand to execute.</param>
		/// <returns>The SqlCommand instance used to execute the query.</returns>
		public static SqlCommand ExecuteNonQuery(string connectionString, SqlCommand command)
		{
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				command.Connection = conn;
				conn.Open();
				command.ExecuteNonQuery();
				return command;
			}
		}
		#endregion

		#region CreateCommand
		/// <summary>
		/// Creates a database connection and creates and returns a SqlCommand instance for that connection. The connection will need 
		/// to be explicitly closed by the calling procedure. This method will execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <returns>A SqlCommand instance with the Connection property set with an open database connection.</returns>
		public static SqlCommand CreateCommand(string connectionString, string query)
		{
			return CreateCommand(connectionString, query, CommandType.StoredProcedure, null);
		}
		/// <summary>
		/// Creates a database connection and creates and returns a SqlCommand instance for that connection. The connection will need 
		/// to be explicitly closed by the calling procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <returns>A SqlCommand instance with the Connection property set with an open database connection.</returns>
		public static SqlCommand CreateCommand(string connectionString, string query, CommandType commandType)
		{
			return CreateCommand(connectionString, query, commandType, null);
		}
		/// <summary>
		/// Creates a database connection and creates and returns a SqlCommand instance for that connection. The connection will need 
		/// to be explicitly closed by the calling procedure. This method will execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>A SqlCommand instance with the Connection property set with an open database connection.</returns>
		public static SqlCommand CreateCommand(string connectionString, string query, params SqlParameter[] parameters)
		{
			return CreateCommand(connectionString, query, CommandType.StoredProcedure, parameters);
		}
		/// <summary>
		/// Creates a database connection and creates and returns a SqlCommand instance for that connection. The connection will need 
		/// to be explicitly closed by the calling procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>A SqlCommand instance with the Connection property set with an open database connection.</returns>
		public static SqlCommand CreateCommand(string connectionString, string query, CommandType commandType, params SqlParameter[] parameters)
		{
			SqlConnection conn = new SqlConnection(connectionString);
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = query;
			cmd.CommandType = commandType;
			cmd.Parameters.AddRange(parameters);
			conn.Open();
			return cmd;
		}
		#endregion

		#region FillDataTable
		/// <summary>
		/// Creates a database connection and fills a DataTable instance with the results of the query. This method will attempt 
		/// to execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <returns>A DataTable filled with the results of the query.</returns>
		public static DataTable FillDataTable(string connectionString, string query)
        {
            return FillDataTable(connectionString, query, (string)null, CommandType.StoredProcedure, null);
        }
		/// <summary>
		/// Creates a database connection and fills a DataTable instance with the results of the query. 
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <returns>A DataTable filled with the results of the query.</returns>
		public static DataTable FillDataTable(string connectionString, string query, CommandType commandType)
		{
			return FillDataTable(connectionString, query, (string)null, commandType, null);
		}
		/// <summary>
		/// Creates a database connection and fills a DataTable instance with the results of the query. This method will attempt 
		/// to execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>A DataTable filled with the results of the query.</returns>
		public static DataTable FillDataTable(string connectionString, string query, params SqlParameter[] parameters)
		{
			return FillDataTable(connectionString, query, null, CommandType.StoredProcedure, parameters);
		}
		/// <summary>
		/// Creates a database connection and fills a DataTable instance with the results of the query. 
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>A DataTable filled with the results of the query.</returns>
		public static DataTable FillDataTable(string connectionString, string query, CommandType commandType, params SqlParameter[] parameters)
		{
			return FillDataTable(connectionString, query, null, commandType, parameters);
		}
		/// <summary>
		/// Creates a database connection and fills a DataTable instance with the results of the query. This method will attempt 
		/// to execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="cacheKey">The key used with SqlCacheDependency. ** NOTE: Not Implemented.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>A DataTable filled with the results of the query.</returns>
        public static DataTable FillDataTable(string connectionString, string query, string cacheKey, params SqlParameter[] parameters)
		{
			return FillDataTable(connectionString, query, cacheKey, CommandType.StoredProcedure, parameters);
		}
		/// <summary>
		/// Creates a database connection and fills a DataTable instance with the results of the query.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="cacheKey">The key used with SqlCacheDependency. ** NOTE: Not Implemented.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>A DataTable filled with the results of the query.</returns>
		public static DataTable FillDataTable(string connectionString, string query, string cacheKey, CommandType commandType, params SqlParameter[] parameters)
		{
			DataTable dt = new DataTable();
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
				{
					da.SelectCommand.CommandType = commandType;
					if (parameters != null && parameters.Length > 0)
					{
						da.SelectCommand.Parameters.AddRange(parameters);
					}
					conn.Open();
					da.Fill(dt);

					//SqlCacheDependency dependency = new SqlCacheDependency();
				}
			}
			return dt;
		}
		#endregion

		#region FillDataSet
		/// <summary>
		/// Creates a database connection and fills a DataSet instance with the results of the query. This method will attempt 
		/// to execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <returns>A DataSet filled with the results of the query.</returns>
		public static DataSet FillDataSet(string connectionString, string query)
        {
            return FillDataSet(connectionString, query, CommandType.StoredProcedure, null);
        }
		/// <summary>
		/// Creates a database connection and fills a DataSet instance with the results of the query.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <returns>A DataSet filled with the results of the query.</returns>
		public static DataSet FillDataSet(string connectionString, string query, CommandType commandType)
		{
			return FillDataSet(connectionString, query, commandType, null);
		}
		/// <summary>
		/// Creates a database connection and fills a DataSet instance with the results of the query. This method will attempt 
		/// to execute the query as a stored procedure.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>A DataSet filled with the results of the query.</returns>
		public static DataSet FillDataSet(string connectionString, string query, params SqlParameter[] parameters)
		{
			return FillDataSet(connectionString, query, CommandType.StoredProcedure, parameters);
		}
		/// <summary>
		/// Creates a database connection and fills a DataSet instance with the results of the query.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to query.</param>
		/// <param name="query">The query text or stored procedure name to execute.</param>
		/// <param name="commandType">One of CommandType values that will be used for the underlying SqlCommand instance.</param>
		/// <param name="parameters">An array of SqlParameter instances.</param>
		/// <returns>A DataSet filled with the results of the query.</returns>
		public static DataSet FillDataSet(string connectionString, string query, CommandType commandType, params SqlParameter[] parameters)
		{
			DataSet ds = new DataSet();
			using (SqlConnection conn = new SqlConnection(connectionString))
			{
				using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
				{
					da.SelectCommand.CommandType = commandType;
					if (parameters != null && parameters.Length > 0)
					{
						da.SelectCommand.Parameters.AddRange(parameters);
					}
					conn.Open();
					da.Fill(ds);
				}
			}
			return ds;
		}
		#endregion

		#endregion
    }
}

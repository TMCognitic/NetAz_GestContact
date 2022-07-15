using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Database
{
    public static class DbConnectionExtensions
    {
        public static object? ExecuteScalar(this IDbConnection dbConnection, string query, bool isStoredProcedure = false, object? parameters = null)
        {
            using(IDbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters))
            {
                if(dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();

                object? result = dbCommand.ExecuteScalar();
                return result is DBNull ? null : result;
            }
        }

        public static int ExecuteNonQuery(this IDbConnection dbConnection, string query, bool isStoredProcedure = false, object? parameters = null)
        {
            using (IDbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters))
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();

                return dbCommand.ExecuteNonQuery();
            }
        }

        public static IEnumerable<TResult> ExecuteReader<TResult>(this IDbConnection dbConnection, string query, Func<IDataRecord, TResult> selector, bool isStoredProcedure = false, object? parameters = null)
        {
            ArgumentNullException.ThrowIfNull(selector);

            using (IDbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters))
            {
                if (dbConnection.State == ConnectionState.Closed)
                    dbConnection.Open();

                using (IDataReader dataReader = dbCommand.ExecuteReader())
                {
                    while(dataReader.Read())
                    {
                        yield return selector(dataReader);
                    }
                }
            }
        }

        private static IDbCommand CreateCommand(IDbConnection dbConnection, string query, bool isStoredProcedure, object? parameters)
        {
            ArgumentNullException.ThrowIfNull(dbConnection);
            ArgumentNullException.ThrowIfNull(query);

            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = query;
            if(isStoredProcedure)
                dbCommand.CommandType = CommandType.StoredProcedure;

            if(parameters is not null)
            {
                Type objectType = parameters.GetType();

                foreach(PropertyInfo propertyInfo in objectType.GetProperties())
                {
                    IDataParameter dataParameter = dbCommand.CreateParameter();
                    dataParameter.ParameterName = propertyInfo.Name;
                    dataParameter.Value = propertyInfo.GetValue(parameters) ?? DBNull.Value;
                    dbCommand.Parameters.Add(dataParameter);
                }
            }

            return dbCommand;
        }
    }
}

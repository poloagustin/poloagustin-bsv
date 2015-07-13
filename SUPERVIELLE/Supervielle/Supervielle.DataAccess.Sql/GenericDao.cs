using Supervielle.DataAccess.Sql.Helpers;
using Supervielle.DataAccess.Sql.Interfaces;
using Supervielle.Domain;
using Supervielle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Sql
{
    public abstract class GenericDao<T> : IGenericDao<T> where T : new()
    {
        protected SqlMapping fields;
        protected abstract string tableName { get; }

        public GenericDao()
        {
            this.fields = SqlMappingsSingleton.GetSqlMapping(this.tableName);
        }

        public GenericDao(SqlMapping fields)
        {
            this.fields = fields;
        }

        public virtual IEnumerable<T> RetrieveRecords(string query)
        {
            var list = new List<T>();
            var connectionString = ConfigurationManager.ConnectionStrings[Properties.Settings.Default.ConnectionStringName].ConnectionString;
            var connection = new SqlConnection(connectionString);
            connection.Open();

            try
            {
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                for (int i = 0; reader.Read(); i++)
                {
                    list.Add(MapToObject(reader));
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return list;
        }

        protected virtual T MapToObject(SqlDataReader reader)
        {
            var obj = new T();
            var type = typeof(T);
            var props = type.GetProperties();

            foreach (var field in this.fields.Attributes)
            {
                try
                {
                    if (props.Any(x => x.Name == field.Property))
                    {
                        props.First(x => x.Name == field.Property).SetValue(obj, GetValue(field, reader));
                    }

                }
                catch (Exception ex)
                {
                    var fieldException = new Exception("Operation failed while reading the field " + field.Property + " of expected type " + field.OriginType.ToString() , ex);
                    throw fieldException;
                }
            }

            return obj;
        }

        private object GetValue(Domain.Attribute field, SqlDataReader reader)
        {
            switch (field.OriginType)
            {
                case OriginType.Byte:
                    return reader.IsDBNull(field.Index) ? default(bool) : Convert.ToBoolean(reader.GetByte(field.Index));
                case OriginType.Short:
                    return reader.IsDBNull(field.Index) ? default(int) : reader.GetInt16(field.Index);
                case OriginType.Int32:
                    return reader.IsDBNull(field.Index) ? default(int) : reader.GetInt32(field.Index);
                case OriginType.String:
                    return reader.IsDBNull(field.Index) ? default(string) : reader.GetString(field.Index);
                case OriginType.Date:
                    return reader.IsDBNull(field.Index) ? default(DateTime) : DateTime.ParseExact(reader.GetInt32(field.Index).ToString(), "yyyyMMdd", null);
                case OriginType.Char:
                    return reader.IsDBNull(field.Index) ? default(char) : reader.GetString(field.Index).FirstOrDefault();
                case OriginType.Decimal:
                    return reader.IsDBNull(field.Index) ? default(decimal) : reader.GetDecimal(field.Index);
                default:
                    return default(string);
            }
        }

        public virtual IEnumerable<T> RetrieveRecords()
        {
            var selectStatement = BuildSelectStatement();
            var query = selectStatement + BuildFromStatement();

            return this.RetrieveRecords(query);
        }

        private string BuildFromStatement()
        {
            return " FROM " + this.tableName;
        }

        private string BuildSelectStatement(string alias = "")
        {
            if (this.fields != null && this.fields.Attributes.Any())
            {
                var firstField = this.fields.Attributes.First();
                var builder = new StringBuilder();
                builder.Append("SELECT ");
                
                foreach (var field in this.fields.Attributes)
                {
                    if (field != firstField)
                    {
                        builder.Append(",");
                    }

                    if (!string.IsNullOrWhiteSpace(alias))
                    {
                        builder.Append(alias);
                        builder.Append(".");
                    }

                    builder.Append(field.Column);
                }

                return builder.ToString();
            }
            else
            {
                throw new Exception("The SqlMapping property is empty.");
            }
        }
    }
}

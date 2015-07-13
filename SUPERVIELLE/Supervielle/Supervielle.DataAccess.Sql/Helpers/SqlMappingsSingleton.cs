using Newtonsoft.Json;
using Supervielle.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.DataAccess.Sql.Helpers
{
    public class SqlMappingsSingleton
    {
        private volatile static object lockObj = new object();
        private static List<SqlMapping> instance;

        public static List<SqlMapping> Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = BuildInstance();
                    }
                }

                return instance;
            }
        }

        private SqlMappingsSingleton() { }

        private static List<SqlMapping> BuildInstance()
        {
            return JsonConvert.DeserializeObject<List<SqlMapping>>(File.ReadAllText(Properties.Settings.Default.SqlMappingsFilePath));
        }

        public static SqlMapping GetSqlMapping(string table)
        {
            return Instance.FirstOrDefault(x => x.Table == table);
        }
    }
}

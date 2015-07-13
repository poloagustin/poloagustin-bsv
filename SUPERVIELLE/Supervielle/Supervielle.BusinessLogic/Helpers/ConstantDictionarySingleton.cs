using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supervielle.BusinessLogic.Helpers
{
    public class ConstantDictionarySingleton
    {
        private volatile static object lockObj = new object();
        private static Dictionary<string, string> instance;

        private ConstantDictionarySingleton()
        {

        }

        public static Dictionary<string, string> Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = BuildContantDictionary();
                    }

                    return instance;
                }
            }
        }

        private static Dictionary<string, string> BuildContantDictionary()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Properties.Settings.Default.ConstantDictionaryFilePath));
        }
    }
}

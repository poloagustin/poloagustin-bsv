using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Accendo.DynamicsIntegration.Crm2015
{
    public class CrmHelperSingleton
    {
        private const string instanceContextKey = "CrmHelper";

        private static CrmHelper instance = null;

        private static object lockObj = new object();

        private CrmHelperSingleton() { }

        public static CrmHelper Instance
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    lock (lockObj)
                    {
                        if (!HttpContext.Current.Items.Contains(instanceContextKey))
                        {
                            HttpContext.Current.Items.Add(instanceContextKey, new CrmHelper());
                        }
                    }

                    return (CrmHelper)HttpContext.Current.Items[instanceContextKey];
                }
                else
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new CrmHelper();
                        }
                    }
                    return instance;
                }
            }
        }
    }
}

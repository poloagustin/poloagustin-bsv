using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Supervielle.DataAccess.Crm
{
    public class MasterDataCache : Dictionary<string, List<Entity>>
    {
        private const string instanceContextKey = "MasterDataCache";

        private static MasterDataCache instance = null;

        private static object lockObj = new object();

        private MasterDataCache() { }

        public static MasterDataCache Instance
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    lock (lockObj)
                    {
                        if (!HttpContext.Current.Items.Contains(instanceContextKey))
                        {
                            HttpContext.Current.Items.Add(instanceContextKey, new MasterDataCache());
                        }
                    }

                    return (MasterDataCache)HttpContext.Current.Items[instanceContextKey];
                }
                else
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new MasterDataCache();
                        }
                    }
                    return instance;
                }
            }
        }

        public void AddItem(string key, Entity item)
        {
            CreateEntry(key);

            (this[key]).Add(item);
        }

        private void CreateEntry(string key)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, new List<Entity>());
            }
        }

        public void RemoveItem(string key, Entity item)
        {
            CreateEntry(key);

            if (this[key].Contains(item))
            {
                this[key].Remove(item);
            }
        }

        public static void Refresh()
        {
            instance = null;
        }

        public Entity GetItem(string key, Func<Entity, bool> predicate)
        {
            CreateEntry(key);

            return this[key].FirstOrDefault(predicate);
        }
    }
}

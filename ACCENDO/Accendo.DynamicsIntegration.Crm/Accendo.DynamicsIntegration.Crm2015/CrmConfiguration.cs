using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accendo.DynamicsIntegration.Crm2015
{
    public class CrmConfiguration
    {
        private CrmConfiguration()
        {
            this.ServerAddress = Properties.Settings.Default.ServerAddress;
            this.OrganizationName = Properties.Settings.Default.OrganizationName;
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.DiscoveryUri))
            {
                this.DiscoveryUri = new Uri(Properties.Settings.Default.DiscoveryUri);
            }
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.OrganizationUri))
            {
                this.OrganizationUri = new Uri(Properties.Settings.Default.OrganizationUri);
            }
            if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.HomeRealmUri))
            {
                this.HomeRealmUri = new Uri(Properties.Settings.Default.HomeRealmUri);
            }
            this.UserName = Properties.Settings.Default.UserName;
            this.Password = Properties.Settings.Default.Password;
            this.Domain = Properties.Settings.Default.Domain;
            this.UserPrincipalName = Properties.Settings.Default.UserPrincipalName;
            this.EndpointType = Properties.Settings.Default.EndpointType;
        }

        public string ServerAddress { get; set; }

        public string OrganizationName { get; set; }

        public Uri DiscoveryUri { get; set; }

        public Uri OrganizationUri { get; set; }

        public Uri HomeRealmUri { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Domain { get; set; }

        public string UserPrincipalName { get; set; }

        public string EndpointType { get; set; }

        private static object lockObj = new object();

        private static CrmConfiguration instance = null;

        public static CrmConfiguration Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (instance == null)
                    {
                        instance = new CrmConfiguration();
                    }
                }

                return instance;
            }
        }
    }
}

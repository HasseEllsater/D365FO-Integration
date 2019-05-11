using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CustomServiceTestUtil
{
    [Serializable]
    public class ServerSettings
    {
        public string AADTenant { get; set; }
        public string AzureAuthEndpoint { get; set; }
        public string Ax7Endpoint { get; set; }
        public string WebAppId { get; set; }
        public string WebAADKey { get; set; }
        public Accent SelectedAccent { get; set; }
        public KeyValuePair<string, Color> SelectedColor { get; set; }
        public bool UseWindowsAccent { get; set; }
        public bool OverrideColorSettings { get; set; }
        public Nullable<double> TimeOut { get; set; }

    }
}

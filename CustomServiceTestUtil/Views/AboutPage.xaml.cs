using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace CustomServiceTestUtil.Views
{
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
            DirSearch();
        }

        public void DirSearch()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string versionInfo = default;
            try
            {
                foreach (string f in Directory.GetFiles(path))
                {
                    try
                    {
                        if (f.EndsWith(".exe") || f.EndsWith(".dll"))
                        {
                            Assembly a = Assembly.LoadFrom(f);
                            versionInfo += string.Format("Name: {0}{1}", AssemblyTitle(a), Environment.NewLine);
                            versionInfo += string.Format("Version: {0}{1}", AssemblyVersion(a), Environment.NewLine);
                            versionInfo += string.Format("Description: {0}{1}", AssemblyDescription(a), Environment.NewLine);
                            versionInfo += string.Format("Product: {0}{1}", AssemblyProduct(a), Environment.NewLine);
                            versionInfo += string.Format("{0}{1}", AssemblyCopyright(a), Environment.NewLine);
                            versionInfo += string.Format("Trademark: {0}{1}", AssemblyTradeMark(a), Environment.NewLine);
                            versionInfo += string.Format("{0}{0}", Environment.NewLine);
                        }
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            catch (Exception)
            {

            }

            VersionInfo.Text = versionInfo;
        }
        private string AssemblyTitle(Assembly a)
        {
            // Get all Title attributes on this assembly
            object[] attributes = a.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            // If there is at least one Title attribute
            if (attributes.Length > 0)
            {
                // Select the first one
                var titleAttribute = (AssemblyTitleAttribute)attributes[0];
                // If it is not an empty string, return it
                if (!string.IsNullOrEmpty(titleAttribute.Title))
                    return titleAttribute.Title;
            }
            // If there was no Title attribute, or if the Title attribute was the empty string, return the .exe name
            return Path.GetFileNameWithoutExtension(a.CodeBase);
        }
        private string AssemblyVersion(Assembly a)
        {
            return a.GetName().Version.ToString();
        }

        private string AssemblyDescription(Assembly a)
        {
            // Get all Description attributes on this assembly
            object[] attributes = a.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            // If there aren't any Description attributes, return an empty string
            if (attributes.Length == 0)
                return "";
            // If there is a Description attribute, return its value
            return ((AssemblyDescriptionAttribute)attributes[0]).Description;
        }

        private string AssemblyProduct(Assembly a)
        {
            // Get all Product attributes on this assembly
            object[] attributes = a.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            // If there aren't any Product attributes, return an empty string
            if (attributes.Length == 0)
                return string.Empty;
            // If there is a Product attribute, return its value
            return ((AssemblyProductAttribute)attributes[0]).Product;
        }

        public string AssemblyCopyright(Assembly a)
        {
            // Get all Copyright attributes on this assembly
            object[] attributes = a.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            // If there aren't any Copyright attributes, return an empty string
            if (attributes.Length == 0)
                return string.Empty;
            // If there is a Copyright attribute, return its value
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }
        public string AssemblyTradeMark(Assembly a)
        {
            // Get all Copyright attributes on this assembly
            object[] attributes = a.GetCustomAttributes(typeof(AssemblyTrademarkAttribute), false);
            // If there aren't any Copyright attributes, return an empty string
            if (attributes.Length == 0)
                return string.Empty;
            // If there is a Copyright attribute, return its value
            return ((AssemblyTrademarkAttribute)attributes[0]).Trademark;
        }
        public string AssemblyCompany(Assembly a)
        {
            // Get all Company attributes on this assembly
            object[] attributes = a.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            // If there aren't any Company attributes, return an empty string
            if (attributes.Length == 0)
                return string.Empty;
            // If there is a Company attribute, return its value
            return ((AssemblyCompanyAttribute)attributes[0]).Company;
        }

     
    }
}

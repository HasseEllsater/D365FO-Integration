using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace ServiceTestInstallerUtil
{
    [RunInstaller(true)]
    public partial class ConfigInstaller : System.Configuration.Install.Installer
    {


        public string ProgramData
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
        }

        public string UtilFolder
        {
            get
            {
                return Path.Combine(ProgramData, Properties.Resources.ApplicationName);
            }
        }

        public string LogFile
        {
            get
            {
                return Path.Combine(UtilFolder, Properties.Resources.SetupLogFile);
            }
        }

        public ConfigInstaller()
        {
            InitializeComponent();
        }
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

        }
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            string startInstallation;
            startInstallation = string.Format(String.Format("Start installation - {0:f}", DateTime.Now));
            Log(startInstallation);
            this.PerformConfigDocumentsSetup();
  
        }
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        private Stream getFile(string Name)
        {
            string asmname = string.Empty;
            try
            {
                // Gets the current assembly.
                Assembly Asm = Assembly.GetExecutingAssembly();
                asmname = Asm.GetName().Name + "." + Name;
                // Resources are named using a fully qualified name.
                Stream strm = Asm.GetManifestResourceStream(asmname);
                return strm; ;
            }
            catch (Exception ex)
            {
                Log(string.Format(Properties.Resources.StreamException, ex.ToString(), asmname));
                throw ex;
            }
        }
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        private void SaveStreamToFile(string fileFullPath, Stream stream)
        {
            if (stream.Length == 0)
            {
                Log(string.Format(Properties.Resources.MissingResource, fileFullPath));
                return;
            }

            // Create a FileStream object to write a stream to a file
            using (FileStream fileStream = System.IO.File.Create(fileFullPath, (int)stream.Length))
            {
                // Fill the bytes[] array with the stream data
                byte[] bytesInStream = new byte[stream.Length];
                stream.Read(bytesInStream, 0, (int)bytesInStream.Length);

                // Use FileStream object to write to the specified file
                fileStream.Write(bytesInStream, 0, bytesInStream.Length);
            }
            Log(string.Format(Properties.Resources.FileCreated, fileFullPath));

        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public void PerformConfigDocumentsSetup()
        {
            Log("PerformConfigDocumentsSetup - start");

            this.RenamePrevFilesBeforeConvert();

            if (!Directory.Exists(UtilFolder))
            {
                Directory.CreateDirectory(UtilFolder);
                this.Log(string.Format(Properties.Resources.DirectoryCreated, UtilFolder));
            }

      
            Stream file = default;


            string fileName = "ServicesAPIV2.json";
            string destfile = Path.Combine(UtilFolder, fileName);
            if(!File.Exists(destfile))
            {
                file = this.getFile(fileName);
                this.SaveStreamToFile(destfile, file);
                this.Log(string.Format("Copied file {0}", fileName));
            }
            else
            {
                this.Log(string.Format("The file {0} already exist, no action performed", fileName));
            }

            fileName = "TestServerSettings.json";
            destfile = Path.Combine(UtilFolder, fileName);
            if(!File.Exists(destfile))
            {
                file = this.getFile(fileName);
                this.SaveStreamToFile(destfile, file);
                this.Log(string.Format("Copied file {0}", fileName));
            }
            else
            {
                this.Log(string.Format("The file {0} already exist, no action performed", fileName));
            }
        }

        private void RenamePrevFilesBeforeConvert()
        {
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
        public void Log(string str)
        {
            StreamWriter Tex;
            try
            {
                Tex = File.AppendText(LogFile);
                Tex.WriteLine(DateTime.Now.ToString() + " " + str);
                Tex.Close();
            }
            catch
            { }
        }
    }
}

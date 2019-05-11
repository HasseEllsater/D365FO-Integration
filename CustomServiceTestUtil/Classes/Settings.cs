using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CustomServiceTestUtil
{
    public class Settings
    {

        public static string ResponseTmpFile
        {
            get
            {
                string programData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                programData = Path.Combine(programData, Properties.Resources.ApplicationName);
                string path = Path.Combine(programData, Properties.Resources.ResponseTmpFile);
                return path;
            }
        }
        public static string TestServerSettings
        {
            get
            {
                string programData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                programData = Path.Combine(programData, Properties.Resources.ApplicationName);
                string path = Path.Combine(programData, Properties.Resources.TestServerSettings);
                return path;
            }
        }
        public static string Endpoints
        {
            get
            {
                string programData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                programData = Path.Combine(programData, Properties.Resources.ApplicationName);
                string path = Path.Combine(programData, Properties.Resources.EndPoints);
                return path;

            }
        }
        public static string ServiceAPIPath
        {
            get
            {
                string programData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                programData = Path.Combine(programData, Properties.Resources.ApplicationName);
                string path = Path.Combine(programData, Properties.Resources.ServiceAPI);
                return path;

            }
        }
        public static string SchemaPath
        {
            get
            {
                string schemaPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                schemaPath = Path.Combine(schemaPath, Properties.Resources.MyDocumentsFolderName);
                return Path.Combine(schemaPath, Properties.Resources.SchemaFolder);
        
            }
        }
        public static string ResponsePath
        {
            get
            {
                string responsePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                responsePath = Path.Combine(responsePath, Properties.Resources.MyDocumentsFolderName);
                return Path.Combine(responsePath, Properties.Resources.ResponseFolder);
        
            }
        }
        public static string ValidationPath
        {
            get
            {
                string validationPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                validationPath = Path.Combine(validationPath, Properties.Resources.MyDocumentsFolderName);
                return Path.Combine(validationPath, Properties.Resources.ValidationFolder);

            }
        }
        public static ServerSettings GetServerSettings()
        {

            ServerSettings serverSettings = new ServerSettings();
      
            string path = Settings.TestServerSettings;

            if (File.Exists(path))
            {
                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serverSettings = (ServerSettings)serializer.Deserialize(file, typeof(ServerSettings));
                }
            }
            return serverSettings;
        }
        public static void SaveServerSettings(ServerSettings _serverSettings)
        {
            string path = Settings.TestServerSettings;
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, _serverSettings);
            }
        }
        public static ObservableCollection<AX7Endpoints> GetEndPointList()
        {
            ObservableCollection<AX7Endpoints> endPointList = new ObservableCollection<AX7Endpoints>();

            string path = Settings.Endpoints;

            if (File.Exists(path))
            {

                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    endPointList = (ObservableCollection<AX7Endpoints>)serializer.Deserialize(file, typeof(ObservableCollection<AX7Endpoints>));
                }
            }
            endPointList.BubbleSort();
            return endPointList;
        }
        public static void SaveEndPoints(ObservableCollection<AX7Endpoints> _EndPointsList)
        {
            string path = Settings.Endpoints;

            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, _EndPointsList);
            }
        }

         public static ListOfServices GetServiceList()
        {
            ListOfServices serviceList = new ListOfServices();

            string path = Settings.ServiceAPIPath;

            if (File.Exists(path))
            {

                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serviceList = (ListOfServices)serializer.Deserialize(file, typeof(ListOfServices));
                }
            }

            serviceList.Services.BubbleSort();
            return serviceList;

        }
        public static void SaveServiceList(ListOfServices _services)
        {
            string path = Settings.ServiceAPIPath;
            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, _services);
            }

        }

        public static ListOfServices AddNestedArray(XmlDocument _doc, ListOfServices _servicesList)
        {
            string serviceName = string.Empty;
            string argument = string.Empty;
            string value = string.Empty;

            _doc.IterateThroughAllNodes(
                delegate (XmlNode node)
                {
                    if (node.Name == "Service")
                    {
                        if (serviceName != node.InnerText)
                        {
                            serviceName = node.InnerText;
                        }
                    }
                    if (node.Name == "Parameter")
                    {
                        argument = node.InnerText;
                    }
                    if (node.Name == "Value")
                    {
                        value = node.InnerText;
                    }
                    if (value != string.Empty)
                    {
                        _servicesList = SetArguments(serviceName, argument, value, _servicesList);
                        value = string.Empty;
                        argument = string.Empty;
                    }
                });


            return _servicesList;
        }
        public static ListOfServices SetArguments(string _service,string _argument,string _value, ListOfServices _servicesList)
        {
            foreach (ServicesAPI service in _servicesList.Services)
            {
                if (service.Service == _service)
                {
                    if(service.Arguments == null)
                    {
                        ObservableCollection<ServiceMethod> Arguments = new ObservableCollection<ServiceMethod>();
                        service.Arguments = Arguments;// APIarguments;
                    }
                    ServiceMethod method = new ServiceMethod
                    {
                        Parameter = _argument,
                        Value = _value
                    };
                    service.Arguments.Add(method);
                }
            }
            return _servicesList;
        }

    }
}

using System.Collections.ObjectModel;

namespace MeasureDataJson
{
    public class JSONSchemaInfo
    {
        public string FullPath { get; set; }
        public string SchemaFile { get; set; }

        public override string ToString()
        {
            return SchemaFile;
        }
    }
    public class JSONSchemas
    {

        private ObservableCollection<JSONSchemaInfo> schemas;

        public ObservableCollection<JSONSchemaInfo> Schemas
        {
            set
            {
                schemas = value;
            }
            get
            {
                if (schemas == null)
                {
                    schemas = new ObservableCollection<JSONSchemaInfo>();
                }
                return schemas;
            }
        }
    }
}

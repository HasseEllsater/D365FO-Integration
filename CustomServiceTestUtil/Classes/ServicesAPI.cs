using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CustomServiceTestUtil
{


    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public class ListOfServices
    {
        private ObservableCollection<ServicesAPI> services;
        public ObservableCollection<ServicesAPI> Services
        {
            get
            {
                if(services == null)
                {
                    services = new ObservableCollection<ServicesAPI>();
                }
                return services;
            }
            set
            {
                services = value;
            }
        }
    }


    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class ServicesAPI :IComparable
    {
        private ObservableCollection<ServiceMethod> arguments;

        public string Group { get; set; }

        public string Service { get; set; }

        public string Method { get; set; }

        public string CallPath { get; set; }

        public CallAction UseParameters {get;set;}

        public ObservableCollection<ServiceMethod> Arguments
        {
            get
            {
                if (arguments == null)
                {
                    arguments = new ObservableCollection<ServiceMethod>();
                }
                return arguments;
            }
            set
            {
                arguments = value;
            }
        }
        public string JSONSchema { get; set; }
        public Direction ValidationDirection { get; set; }

        public int CompareTo(object obj)
        {
            ServicesAPI serviceAPI = obj as ServicesAPI;

            return this.CallPath.CompareTo(serviceAPI.CallPath);
        }

        public override string ToString()
        {
            return CallPath;
        }
    }

    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class ServiceMethod
    {
        public string Parameter { get; set; }
        public string Value { get; set; }

    }

    //Används för att skicka in en lista till propertygrid, går inte att serialisera
    //Behåll som referens, bra skit
    [TypeConverter(typeof(ServiceMethod))]
    [DescriptionAttribute("Metod argument och värden")]
    public class ServiceMethod2 : ExpandableObjectConverter //, ISerializable
    {
        public string Parameter { get; set; }
        public string Value { get; set; }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(ServiceMethod))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is ServiceMethod)
            {
                ServiceMethod method = value as ServiceMethod;

                return method.Parameter;//string.Format("Metod: {0} - Argument: {1}", method.Parameter, method.Value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
 }

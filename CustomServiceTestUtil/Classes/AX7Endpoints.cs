using System;

namespace CustomServiceTestUtil
{
    [Serializable]
    public class AX7Endpoints : IComparable
    {
        public string Machine { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
        public string EndPointURI { get; set; }

        public string URI { get; set; }

        public int CompareTo(object obj)
        {
            AX7Endpoints endpoint = obj as AX7Endpoints;

            return this.Name.CompareTo(endpoint.Name);
        }

        public override string ToString()
        {
            return Name;
        }

    }
}

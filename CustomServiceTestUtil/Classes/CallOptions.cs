using System.ComponentModel;

namespace CustomServiceTestUtil
{
    class Options
    {
        public string Name { get; set; }
        public CallAction SelectedAction { get; set;}

        public override string ToString()
        {
            return Name;
        }

    }
}

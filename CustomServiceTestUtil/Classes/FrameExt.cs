using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CustomServiceTestUtil
{
    public class FrameExt : Frame
    {
        private static Dictionary<string, object> objects = new Dictionary<string, object>();
        public static void SetSharedValue(string _key, object _value)
        {
            objects.Add(_key, _value);
        }
        public static object GetSharedValue(string _key)
        {
            object outValue;
            objects.TryGetValue(_key, out outValue);
            return outValue;
        }
        public static void RemoveSharedValue(string _key)
        {
            objects.Remove(_key);
        }
    }
}

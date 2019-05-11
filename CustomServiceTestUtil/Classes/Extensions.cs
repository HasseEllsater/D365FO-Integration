using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomServiceTestUtil
{
    /// <summary>
    /// Sortera observable collection
    /// </summary>
    public static class ListExtension
    {
        public static void BubbleSort(this IList o)
        {
            for (int i = o.Count - 1; i >= 0; i--)
            {
                for (int j = 1; j <= i; j++)
                {
                    object o1 = o[j - 1];
                    object o2 = o[j];
                    if (((IComparable)o1).CompareTo(o2) > 0)
                    {
                        o.Remove(o1);
                        o.Insert(j, o1);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Använd description attribut på enum för en anpassad text
    /// 
    /// EnumHelper.GetDescription(<enum variabel>.Direction)
    /// 
    /// public enum Direction : int
    /// {
    ///    [Description("-")]
    ///    NotUsed = 0,
    ///    [Description("Set anrop")]
    ///    SET = 1,
    ///    [Description("Get anrop")]
    ///    GET = 2,
    /// }
    /// </summary>
    public static class EnumHelper
    {
        public static string GetDescription(this Enum GenericEnum)
        {

            Type genericEnumType = GenericEnum.GetType();
            System.Reflection.MemberInfo[] memberInfo =
                        genericEnumType.GetMember(GenericEnum.ToString());

            if ((memberInfo != null && memberInfo.Length > 0))
            {

                dynamic _Attribs = memberInfo[0].GetCustomAttributes
                      (typeof(System.ComponentModel.DescriptionAttribute), false);
                if ((_Attribs != null && _Attribs.Length > 0))
                {
                    return ((System.ComponentModel.DescriptionAttribute)_Attribs[0]).Description;
                }
            }

            return GenericEnum.ToString();
        }

    }
}

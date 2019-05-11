using MahApps.Metro;
using Microsoft.Win32;
using System;
using System.Windows;

namespace CustomServiceTestUtil
{
    public static class ThemeHelper
    {
        public enum ThemeChoices
        {
            Light = 0,
            Dark = 1
        }

        private static ThemeChoices GetCurrentTheme()
        {
            using (RegistryKey rv = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize"))
            {
                if (rv != null)
                {
                    var value = rv.GetValue("AppsUseLightTheme");
                    if (value != null)
                    {
                        UInt32 lightTheme = (UInt32)(int)value;
                        if (lightTheme == 0)
                        {
                            return ThemeChoices.Dark;
                        }
                    }
                }
            }

            return ThemeChoices.Light;
        }

        private static int GetDiff(System.Windows.Media.Color clr, Accent item2)
        {
            System.Windows.Media.Color clr2 = (System.Windows.Media.Color)item2.Resources["AccentColor"];
            return GetDiff(System.Drawing.Color.FromArgb(clr.A, clr.R, clr.G, clr.B), System.Drawing.Color.FromArgb(clr2.A, clr2.R, clr2.G, clr2.B));
        }

        private static int GetDiff(System.Drawing.Color color, System.Drawing.Color baseColor)
        {
            int a = color.A - baseColor.A,
                r = color.R - baseColor.R,
                g = color.G - baseColor.G,
                b = color.B - baseColor.B;
            return a * a + r * r + g * g + b * b;
        }
        public static void ChooseTheme()
        {
            System.Windows.Media.Color clr = SystemParameters.WindowGlassColor;
            using (var rk = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\DWM"))
            {
                var p = rk.GetValue("ColorizationColor", 0);
                if (p != null)
                {
                    UInt32 pp = (UInt32)(int)p;
                    if (pp > 0)
                    {
                        var bytes = BitConverter.GetBytes(pp);
                        clr = System.Windows.Media.Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
                    }
                }
            }

            Tuple<AppTheme, Accent> appStyle = ThemeManager.DetectAppStyle(Application.Current);
            Tuple<Accent, int> newAccent = new Tuple<Accent, int>(appStyle.Item2, GetDiff(clr, appStyle.Item2));
            foreach (var accent in ThemeManager.Accents)
            {
                int a = GetDiff(clr, accent);
                if (a < newAccent.Item2)
                {
                    newAccent = new Tuple<Accent, int>(accent, a);
                }
            }
            var application = Application.Current;
            ThemeManager.ChangeAppStyle(application, newAccent.Item1, (GetCurrentTheme() == ThemeChoices.Dark) ? ThemeManager.GetAppTheme("BaseDark") : ThemeManager.GetAppTheme("BaseLight"));
        }
    }
}
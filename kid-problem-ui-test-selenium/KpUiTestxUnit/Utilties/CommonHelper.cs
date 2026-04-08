using System.Drawing;
using System.Text.RegularExpressions;
using OpenQA.Selenium.BiDi.Emulation;
using OpenQA.Selenium.Internal;

namespace KpUiTestxUnit.Utilties;

public static class CommonHelper
{
    public static string EncodeUrl(string r)
    {
        return r.Replace(" ", "%20");
    }

    public static int? ConvertToInt(string? s)
    {
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }

        if (int.TryParse(s.Trim(), out int result))
        {
            return result;
        }
        return null;
    }

    public static string? RgbaToHex(this string? rgba, bool noAlpha = true)
    {
        if (string.IsNullOrEmpty(rgba))
        {
            return rgba;
        }

        string pattern = @"\((.+?)\)";
        var m = Regex.Match(rgba, pattern);
        if (m.Success)
        {
            var values = m.Groups[1].Value;
            if (string.IsNullOrEmpty(values))
            {
                return rgba;
            }

            var colors = values.Split(',');
            if (colors.Length < 3)
            {
                return rgba;
            }

            if (!int.TryParse(colors[0], out int r))
            {
                return rgba;
            }
            r = Math.Clamp(r, 0, 255);

            if (!int.TryParse(colors[1], out int g))
            {
                return rgba;
            }
            g = Math.Clamp(g, 0, 255);

            if (!int.TryParse(colors[2], out int b))
            {
                return rgba;
            }
            b = Math.Clamp(b, 0, 255);

            if (!noAlpha && colors.Length > 3 && double.TryParse(colors[3], out double a))
            {
                a = Math.Clamp(a, 0.0, 1.0);
                int alpha = (int)Math.Round(a * 255); 
                return $"#{r:X2}{g:X2}{b:X2}{alpha:X2}";
            }
            else
            {
                return $"#{r:X2}{g:X2}{b:X2}";
            }

        }

        return rgba;
    }
}

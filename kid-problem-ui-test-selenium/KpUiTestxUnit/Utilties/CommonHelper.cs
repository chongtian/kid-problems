using System.Reflection;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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

    /// <summary>
    /// Compare screenshot with the baseline
    /// </summary>
    /// <param name="baselineImageName">Baseline Image Name excluding the extention. eg., baseline, instead of baseline.png</param>
    /// <param name="imageFullNameFromRuntime">Screenshot taken in the runtime. It is the full name of the png file. </param>
    /// <param name="perChannelTolerance">The tolerance allowed for each RGB channel.</param>
    /// <param name="allowedDifferenceRatio"></param>
    /// <returns></returns>
    public static bool ComparePictureWithTolerance(string baselineImageName, string imageFullNameFromRuntime, int perChannelTolerance = 5, double allowedDifferenceRatio = 0.01)
    {

        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream($"KpUiTestxUnit.Data.{baselineImageName}.png");

        if (stream == null)
        {
            Console.WriteLine($"Failed to load KpUiTestxUnit.Data.{baselineImageName}.png");
            return false;
        }

        using var img1 = Image.Load<Rgba32>(stream);
        using var img2 = Image.Load<Rgba32>(imageFullNameFromRuntime);

        if (img1.Width != img2.Width || img1.Height != img2.Height)
        {
            Console.WriteLine("Images must be the same size.");
            return false;
        }

        int diffPixels = 0;
        int totalPixels = img1.Width * img1.Height;

        for (int y = 0; y < img1.Height; y++)
        {
            for (int x = 0; x < img1.Width; x++)
            {
                var p1 = img1[x, y];
                var p2 = img2[x, y];

                if (Math.Abs(p1.R - p2.R) > perChannelTolerance ||
                    Math.Abs(p1.G - p2.G) > perChannelTolerance ||
                    Math.Abs(p1.B - p2.B) > perChannelTolerance)
                {
                    diffPixels++;
                }
            }
        }

        double diffRatio = (double)diffPixels / totalPixels;
        return diffRatio <= allowedDifferenceRatio;
    }
}

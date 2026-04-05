namespace KpUiTestxUnit.Utilties;

public static class CommonHelper
{
    public static string EncodeUrl(string r)
    {
        return r.Replace(" ", "%20");
    }

    public static int? ConvertToInt(string? s)
    {
        if(string.IsNullOrEmpty(s))
        {
            return null;
        }

        if (int.TryParse(s.Trim(), out int result))
        {
            return result;
        }
        return null;
    }
}

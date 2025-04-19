namespace CommonLib.Helpers;

public static class StringHelper
{
    public static string PopulateWithWhiteSpaces(string str, int stringLength)
    {
        var whiteSpacesCount = stringLength - str.Length;
        if (whiteSpacesCount <= 0)
            return str;
        whiteSpacesCount *= 3;
        for (var i = 0; i < whiteSpacesCount; i++)
        {
            str += " ";
        }
        return str;
    }
}
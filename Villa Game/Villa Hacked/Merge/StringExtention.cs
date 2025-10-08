namespace Merge;

public static class StringExtention
{
	public static string SetFontColor(this string str, string color)
	{
		return "<color=" + color + ">" + str + "</color>";
	}

	public static string SetFontColor(this string str, StringColors color)
	{
		return str.SetFontColor(color.ToString());
	}

	public static string SetFontItalics(this string str)
	{
		return "<i>" + str + "</i>";
	}

	public static string SetFontBold(this string str)
	{
		return "<b>" + str + "</b>";
	}

	public static string SetFontSize(this string str, int size)
	{
		return $"<size={size}>{str}</size>";
	}

	public static bool IsNullOrEmpty(this string str)
	{
		return string.IsNullOrEmpty(str);
	}
}

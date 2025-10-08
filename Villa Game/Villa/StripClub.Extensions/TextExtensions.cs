using System.Linq;

namespace StripClub.Extensions;

public static class TextExtensions
{
	public static string StripUnicodeCharactersFromString(this string inputValue)
	{
		return new string(inputValue.Where((char c) => c <= '\u007f').ToArray());
	}
}

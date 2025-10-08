using System.Linq;

namespace StripClub.Utility;

public static class MapperUtility
{
	public static int[] MapperStringToArray(string input, params char[] splitChars)
	{
		return (from _str in input.Split(splitChars)
			select int.Parse(_str)).ToArray();
	}
}

using StripClub.Extensions;

namespace GreenT.Net.User;

public static class UserDataExtension
{
	public static string FormatEmail(this string email)
	{
		return email.Trim().StripUnicodeCharactersFromString().ToLower();
	}

	public static string FormatPassword(this string password)
	{
		return password.Trim().StripUnicodeCharactersFromString();
	}
}

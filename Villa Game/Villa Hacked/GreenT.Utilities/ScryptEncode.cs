using System.Text;
using CryptSharp.Utility;

namespace GreenT.Utilities;

public static class ScryptEncode
{
	private const int N = 16384;

	private const int r = 8;

	private const int p = 1;

	private const int lenght = 128;

	public static string Encode(string password, string salt)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(password);
		byte[] bytes2 = Encoding.UTF8.GetBytes(salt);
		byte[] bytes3 = SCrypt.ComputeDerivedKey(bytes, bytes2, 16384, 8, 1, (int?)null, 128);
		return Encoding.ASCII.GetString(bytes3);
	}
}

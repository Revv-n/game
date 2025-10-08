using System;
using System.Text;

namespace Games.Coresdk.Unity;

public sealed class Base64Util
{
	public static string ToBase64(string value)
	{
		return Convert.ToBase64String(Encoding.GetEncoding("utf-8").GetBytes(value));
	}
}

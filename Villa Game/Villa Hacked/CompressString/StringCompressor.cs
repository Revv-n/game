using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

namespace CompressString;

internal static class StringCompressor
{
	public static string CompressString(string text)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		MemoryStream memoryStream = new MemoryStream();
		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
		{
			gZipStream.Write(bytes, 0, bytes.Length);
		}
		memoryStream.Position = 0L;
		byte[] array = new byte[memoryStream.Length];
		memoryStream.Read(array, 0, array.Length);
		byte[] array2 = new byte[array.Length + 4];
		Buffer.BlockCopy(array, 0, array2, 4, array.Length);
		Buffer.BlockCopy(BitConverter.GetBytes(bytes.Length), 0, array2, 0, 4);
		return Convert.ToBase64String(array2);
	}

	public static bool DecompressString(string compressedText, out string result)
	{
		byte[] array;
		try
		{
			array = Convert.FromBase64String(compressedText);
		}
		catch (Exception ex)
		{
			Debug.Log("text is not base64string. Ex " + ex);
			result = compressedText;
			return false;
		}
		using MemoryStream memoryStream = new MemoryStream();
		int num = BitConverter.ToInt32(array, 0);
		memoryStream.Write(array, 4, array.Length - 4);
		byte[] array2 = new byte[num];
		memoryStream.Position = 0L;
		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
		{
			gZipStream.Read(array2, 0, array2.Length);
		}
		result = Encoding.UTF8.GetString(array2);
		return true;
	}
}

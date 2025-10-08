using System.IO;
using System.IO.Compression;

namespace GreenT.Net;

public static class GZipUtility
{
	public static byte[] Compress(this byte[] data)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress);
		gZipStream.Write(data, 0, data.Length);
		gZipStream.Close();
		return memoryStream.ToArray();
	}

	public static byte[] Compress(this Stream stream)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (new GZipStream(memoryStream, CompressionMode.Compress))
		{
			stream.CopyTo(memoryStream);
			return memoryStream.ToArray();
		}
	}

	public static byte[] Decompress(this byte[] data)
	{
		using MemoryStream compressedStream = new MemoryStream(data);
		using MemoryStream memoryStream = compressedStream.Decompress();
		return memoryStream.ToArray();
	}

	public static MemoryStream Decompress(this Stream compressedStream)
	{
		using GZipStream gZipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
		MemoryStream memoryStream = new MemoryStream();
		gZipStream.CopyTo(memoryStream);
		return memoryStream;
	}
}

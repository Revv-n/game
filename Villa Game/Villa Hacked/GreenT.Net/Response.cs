using System;
using Newtonsoft.Json;

namespace GreenT.Net;

[Serializable]
public class Response<T> : Response
{
	[JsonProperty]
	private T data;

	public T Data => data;

	[JsonConstructor]
	public Response(T data, int status, string error)
		: base(status, error)
	{
		this.data = data;
	}

	public Response(T data, Response response)
		: this(data, response.Status, response.Error)
	{
	}

	public override string ToString()
	{
		return base.ToString() + ((Data != null) ? ("\n" + Data.ToString()) : string.Empty);
	}
}
[Serializable]
public class Response
{
	[JsonProperty]
	private int status;

	[JsonProperty]
	private string error;

	public int Status => status;

	public string Error => error;

	public Response(int status, string error = "")
	{
		this.status = status;
		this.error = error;
	}

	public override string ToString()
	{
		string text = "Response status: " + status;
		if (error != string.Empty)
		{
			text = text + " error: " + error;
		}
		return text;
	}
}

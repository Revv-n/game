using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UniRx;

namespace GreenT.Net;

public class PostRequest<T> : IPostRequest<T>, IPostRequest<T, IDictionary<string, string>>
{
	public readonly string baseUrl;

	protected string url;

	public PostRequest(string url)
	{
		baseUrl = url;
		this.url = baseUrl;
	}

	public virtual IObservable<T> Post(IDictionary<string, string> fields)
	{
		return HttpRequestExecutor.PostRequest(url, fields).Select(JsonConvert.DeserializeObject<T>).Catch(delegate(Exception ex)
		{
			throw ex.SendException(" On waiting object of type " + typeof(T).Name);
		});
	}

	public virtual IObservable<string> PostNoResponse(IDictionary<string, string> fields)
	{
		return HttpRequestExecutor.PostRequest(url, fields).Catch(delegate(Exception ex)
		{
			throw ex.SendException(" On waiting object of type " + typeof(T).Name);
		});
	}

	public IObservable<string> PostNoResponse(IDictionary<string, string> fields, params object[] values)
	{
		url = string.Format(baseUrl, values);
		return PostNoResponse(fields);
	}

	public IObservable<T> Post(IDictionary<string, string> fields, params object[] values)
	{
		url = string.Format(baseUrl, values);
		return Post(fields);
	}

	public IObservable<T> PostWithEmptyFields(params object[] values)
	{
		IDictionary<string, string> fields = new Dictionary<string, string>();
		return Post(fields, values);
	}
}

using System;
using System.Collections.Generic;
using GreenT.Steam;
using Newtonsoft.Json;
using UniRx;

namespace GreenT.Net;

public class PostJsonRequest<TResponse> : PostJsonRequest<TResponse, IDictionary<string, string>>, IPostRequest<TResponse>, IPostRequest<TResponse, IDictionary<string, string>>
{
	public PostJsonRequest(string requestUrl)
		: base(requestUrl)
	{
	}
}
public class PostJsonRequest<TResponse, TValue> : IPostRequest<TResponse, TValue>, ICustomizablePostRequest<TResponse, TValue>
{
	private readonly string requestUrl;

	public PostJsonRequest(string requestUrl)
	{
		this.requestUrl = requestUrl;
	}

	public virtual IObservable<TResponse> Post(TValue fields)
	{
		return RequestByUrl(requestUrl, fields);
	}

	public IObservable<TResponse> Post(TValue fields, params object[] objects)
	{
		string text = ((objects == null || objects.Length == 0) ? requestUrl : string.Format(requestUrl, objects));
		return RequestByUrl(text, fields);
	}

	public IObservable<TResponse> PostWithHeaders(TValue fields, IDictionary<string, string> headers, params object[] objects)
	{
		string text = TryFormatRequestURL(objects);
		return RequestByUrl(text, fields, headers);
	}

	private IObservable<TResponse> RequestByUrl(string requestUrl, TValue fields, IDictionary<string, string> headers = null)
	{
		string jsonSend = JsonConvert.SerializeObject(fields);
		string responseContent = "";
		string deserializedResponseContent = "";
		return HttpRequestExecutor.PostJSONRequest(requestUrl, jsonSend, headers).Select(JsonConvert.DeserializeObject<TResponse>).Catch(delegate(Exception ex)
		{
			throw ex.SendException($"{GetType().Name}: Exception on try to parse response by url : {requestUrl}. Exception is {ex}. Exception message: {ex.Message}. Deserialized response content: {deserializedResponseContent}. AppId {SteamConstants.GetSteamAppID()}. SteamUserID:{SteamConstants.GetUserSteamID()} Exception source: {ex.Source}. Json send: {jsonSend}. Response content: {responseContent}.");
		});
	}

	private string TryFormatRequestURL(params object[] objects)
	{
		if (objects != null && objects.Length != 0)
		{
			return string.Format(requestUrl, objects);
		}
		return requestUrl;
	}
}

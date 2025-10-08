using System;
using GreenT.Net;

namespace GreenT.Settings.Data;

public class GetConfigsVersionRequest
{
	private readonly IRequestUrlResolver _urlResolver;

	private readonly IGetRequest _configurationInfoRequest;

	private Response<ConfigurationInfo> _cachedResponse;

	public GetConfigsVersionRequest(IRequestUrlResolver urlResolver, IGetRequest configurationInfoRequest)
	{
		_urlResolver = urlResolver;
		_configurationInfoRequest = configurationInfoRequest;
	}

	public IObservable<Response<ConfigurationInfo>> Get()
	{
		string requestUrl = _urlResolver.PostRequestUrl(PostRequestType.ConfigVersion);
		return _configurationInfoRequest.GetRequest<Response<ConfigurationInfo>>(requestUrl, Array.Empty<object>());
	}
}

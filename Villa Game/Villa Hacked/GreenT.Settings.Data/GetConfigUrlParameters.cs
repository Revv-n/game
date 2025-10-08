using System;
using GreenT.Net;
using UniRx;

namespace GreenT.Settings.Data;

public class GetConfigUrlParameters : IGetConfigUrlParameters
{
	private readonly GetConfigsVersionRequest getConfigsVersionRequest;

	public GetConfigUrlParameters(GetConfigsVersionRequest getConfigsVersionRequest)
	{
		this.getConfigsVersionRequest = getConfigsVersionRequest;
	}

	public IObservable<ConfigurationInfo> Get()
	{
		return Observable.Select<Response<ConfigurationInfo>, ConfigurationInfo>(getConfigsVersionRequest.Get(), (Func<Response<ConfigurationInfo>, ConfigurationInfo>)((Response<ConfigurationInfo> x) => x.Data));
	}
}

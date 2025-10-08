using System;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class ErolabsGetBalanceRequest : HTTPConcretteGetRequestNoRespone<ErolabsBalanceData>
{
	private string url;

	public ErolabsGetBalanceRequest(string url)
		: base(url)
	{
		this.url = url;
	}

	public new IObservable<ErolabsBalanceData> GetRequest(params object[] args)
	{
		return GetRequestBase(url, args);
	}

	public IObservable<ErolabsBalanceData> GetRequestBase(string requestUrl, params object[] args)
	{
		return HttpRequestExecutor.GetRequest<ErolabsBalanceData>(string.Format(requestUrl, args));
	}
}

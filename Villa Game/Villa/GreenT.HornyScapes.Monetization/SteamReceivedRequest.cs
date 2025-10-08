using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class SteamReceivedRequest : PostJsonRequest<Response>
{
	public SteamReceivedRequest(string url)
		: base(url)
	{
	}

	public IObservable<Response> Post(SteamPaymentData data)
	{
		IDictionary<string, string> fields = new Dictionary<string, string> { 
		{
			"order_id",
			data.order_id.ToString()
		} };
		return base.Post(fields);
	}
}

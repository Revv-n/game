using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class SteamConfirmRequest : PostObjectJsonRequest<SteamPaymentData>
{
	public SteamConfirmRequest(string url)
		: base(url)
	{
	}

	public IObservable<SteamPaymentData> Post(string transaction_id, bool unreceived)
	{
		IDictionary<string, object> fields = new Dictionary<string, object>
		{
			{ "order_id", transaction_id },
			{ "unreceived", unreceived }
		};
		return base.Post(fields);
	}
}

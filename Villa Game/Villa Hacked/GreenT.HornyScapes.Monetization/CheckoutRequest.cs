using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class CheckoutRequest : PostRequest<Response<CheckoutData>>
{
	private readonly string appName;

	public CheckoutRequest(string url, string appName)
		: base(url)
	{
		this.appName = appName;
	}

	public IObservable<Response<CheckoutData>> Post(string playerId, string lotId, string itemId)
	{
		IDictionary<string, string> fields = new Dictionary<string, string>
		{
			{ "player_id", playerId },
			{ "item_id", itemId },
			{ "app_name", appName },
			{ "bundle", lotId }
		};
		return base.Post(fields);
	}
}

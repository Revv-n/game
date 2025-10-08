using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class ErolabsInvoicesFilteredRequest : PostJsonRequest<Response>
{
	private User user;

	public ErolabsInvoicesFilteredRequest(string url, User user)
		: base(url)
	{
		this.user = user;
	}

	public IObservable<Response> Post(string tokenAuth, string itemId)
	{
		IDictionary<string, string> fields = new Dictionary<string, string>
		{
			{ "token_auth", tokenAuth },
			{ "player_id", user.PlayerID },
			{ "item_id", itemId }
		};
		return base.Post(fields);
	}
}

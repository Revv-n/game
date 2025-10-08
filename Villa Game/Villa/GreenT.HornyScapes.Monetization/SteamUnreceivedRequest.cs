using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Monetization;

public class SteamUnreceivedRequest : PostJsonRequest<List<SteamPaymentData>>
{
	private readonly User _user;

	public SteamUnreceivedRequest(string url, User user)
		: base(url)
	{
		_user = user;
	}

	public IObservable<List<SteamPaymentData>> Post()
	{
		IDictionary<string, string> fields = new Dictionary<string, string> { { "player_id", _user.PlayerID } };
		return base.Post(fields);
	}
}

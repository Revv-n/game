using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class LeaderboardAddPointsRequest : PostObjectJsonRequest<Response>
{
	private readonly User _user;

	public LeaderboardAddPointsRequest(string url, User user)
		: base(url)
	{
		_user = user;
	}

	public IObservable<Response> Post(string reason, string token, int amount)
	{
		IDictionary<string, object> fields = new Dictionary<string, object>
		{
			{ "nickname", _user.Nickname },
			{ "reason", reason },
			{ "score", amount }
		};
		IDictionary<string, string> headers = new Dictionary<string, string> { { "Authorization", token } };
		return PostWithHeaders(fields, headers);
	}
}

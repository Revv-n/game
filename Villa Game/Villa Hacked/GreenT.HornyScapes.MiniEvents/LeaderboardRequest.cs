using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class LeaderboardRequest : HTTPSpecializedGetRequest<LeaderboardResponse>
{
	private readonly string requestUrl;

	private readonly AppIdHolder _appIdHolder;

	private readonly User _user;

	public LeaderboardRequest(string url, User user, AppIdHolder appIdHolder)
	{
		requestUrl = url;
		_user = user;
		_appIdHolder = appIdHolder;
	}

	public IObservable<LeaderboardResponse> GetRequest(int range, string token, params string[] args)
	{
		IDictionary<string, string> headers = new Dictionary<string, string> { { "Authorization", token } };
		return GetRequest(string.Format(requestUrl, range), headers, args);
	}
}

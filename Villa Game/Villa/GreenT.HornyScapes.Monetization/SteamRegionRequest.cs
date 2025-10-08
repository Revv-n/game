using System;
using System.Collections.Generic;
using GreenT.Net;
using GreenT.Steam;

namespace GreenT.HornyScapes.Monetization;

public class SteamRegionRequest : PostObjectJsonRequest<SteamUserResponse>
{
	private readonly User _user;

	public SteamRegionRequest(string url, User user)
		: base(url)
	{
		_user = user;
	}

	public IObservable<SteamUserResponse> Post()
	{
		IDictionary<string, object> fields = new Dictionary<string, object>
		{
			{
				"appid",
				SteamConstants.GetSteamAppID()
			},
			{ "steam_id", _user.PlaftormId },
			{
				"is_dev",
				SteamConstants.GetIsDev()
			}
		};
		return base.Post(fields);
	}
}

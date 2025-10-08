using System;
using System.Collections.Generic;
using GreenT.Net;
using GreenT.Steam;

namespace GreenT.HornyScapes.Monetization;

public class SteamCheckoutRequest : PostJsonRequest<Response>
{
	private const int MAX_DESCRIPTION_CHAR = 100;

	private readonly User _user;

	public SteamCheckoutRequest(string url, User user)
		: base(url)
	{
		_user = user;
	}

	public IObservable<Response> Post(string lotId, string region, string monetizationId, string description, string currencyType)
	{
		if (string.IsNullOrEmpty(description) || description.Length >= 100)
		{
			description = "description";
		}
		IDictionary<string, string> fields = new Dictionary<string, string>
		{
			{ "player_id", _user.PlayerID },
			{ "language", "EN" },
			{ "region", region },
			{ "steam_id", _user.PlaftormId },
			{ "item_id", monetizationId },
			{
				"app_id",
				SteamConstants.GetSteamAppID()
			},
			{ "bundle", lotId },
			{ "description", description },
			{ "currencyType", currencyType }
		};
		return base.Post(fields);
	}
}

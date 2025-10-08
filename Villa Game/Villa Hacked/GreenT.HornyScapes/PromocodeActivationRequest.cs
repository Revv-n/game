using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes;

public class PromocodeActivationRequest : PostObjectJsonRequest<PromocodeRewardResponse>
{
	private readonly User user;

	public PromocodeActivationRequest(string url, User user)
		: base(url)
	{
		this.user = user;
	}

	public IObservable<PromocodeRewardResponse> Post(string promocodeName, string platformName)
	{
		IDictionary<string, object> fields = new Dictionary<string, object>
		{
			{ "promocode_name", promocodeName },
			{ "platform_name", platformName },
			{ "player_id", user.PlayerID }
		};
		new Dictionary<string, string>().Add("Content-Type", "application/json");
		return base.Post(fields);
	}
}

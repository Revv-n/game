using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes;

public sealed class RegistrationRequest : PostObjectJsonRequest<RegistrationResponse>
{
	private readonly AppIdHolder _appIdHolder;

	private readonly User _user;

	public RegistrationRequest(string url, User user, AppIdHolder appIdHolder)
		: base(url)
	{
		_user = user;
		_appIdHolder = appIdHolder;
	}

	public IObservable<RegistrationResponse> Post(RatingData targetRatingData)
	{
		string appID = _appIdHolder.AppID;
		IDictionary<string, object> fields = new Dictionary<string, object>
		{
			{
				"platform",
				appID ?? ""
			},
			{ "player_id", _user.PlayerID },
			{ "player_name", _user.Nickname },
			{ "player_power", targetRatingData.PlayerPower },
			{ "calendar_id", targetRatingData.CalendarID }
		};
		return Post(fields);
	}
}

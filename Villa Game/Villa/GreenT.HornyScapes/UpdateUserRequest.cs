using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes;

public sealed class UpdateUserRequest : PostObjectJsonRequest<Response>
{
	private readonly User _user;

	public UpdateUserRequest(string requestUrl, User user)
		: base(requestUrl)
	{
		_user = user;
	}

	public IObservable<Response> Post()
	{
		IDictionary<string, object> fields = new Dictionary<string, object>
		{
			{ "player_id", _user.PlayerID },
			{ "nickname", _user.Nickname },
			{ "email", _user.Email },
			{ "fb_id", _user.FBID },
			{ "platform_id", _user.PlaftormId },
			{ "is_updated", true },
			{ "apple_id", _user.AppleID }
		};
		return Post(fields);
	}
}

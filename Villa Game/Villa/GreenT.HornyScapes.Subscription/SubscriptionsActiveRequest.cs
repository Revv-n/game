using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.Subscription;

public sealed class SubscriptionsActiveRequest : HTTPSpecializedGetRequest<List<SubscriptionResponse>>
{
	private readonly string _requestUrl;

	private readonly User _user;

	public SubscriptionsActiveRequest(string requestUrl, User user)
	{
		_requestUrl = requestUrl;
		_user = user;
	}

	public IObservable<List<SubscriptionResponse>> GetRequest(params string[] args)
	{
		return GetRequest(string.Format(_requestUrl, _user.PlayerID), null, args);
	}
}

using System;
using System.Collections.Generic;
using GreenT.Net;

namespace GreenT.HornyScapes.AntiCheat;

public sealed class UpdateCheatingStatusRequest
{
	[Serializable]
	public sealed class CheaterResponse
	{
		public bool IsCheater;
	}

	private readonly User _user;

	private readonly string _requestUrl;

	public UpdateCheatingStatusRequest(string requestUrl, User user)
	{
		_requestUrl = requestUrl;
		_user = user;
	}

	public IObservable<CheaterResponse> GetRequest(bool isCheater)
	{
		if (!isCheater)
		{
			return null;
		}
		string requestUrl = string.Format(_requestUrl, _user.PlayerID);
		return GetRequest(requestUrl);
	}

	private IObservable<CheaterResponse> GetRequest(string requestUrl, IEnumerable<KeyValuePair<string, string>> headers = null)
	{
		return HttpRequestExecutor.GetRequest<CheaterResponse>(requestUrl, cached: false, headers);
	}
}

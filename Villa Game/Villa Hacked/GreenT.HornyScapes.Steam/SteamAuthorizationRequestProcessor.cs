using System.Collections.Generic;
using GreenT.Net;
using GreenT.Net.User;
using GreenT.Steam;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Steam;

public class SteamAuthorizationRequestProcessor : BaseAuthorizationRequestProcessor
{
	public SteamAuthorizationRequestProcessor(User userData, [Inject(Id = "AuthorizationData")] IPostRequest<Response<UserDataMapper>> postRequest)
		: base(postRequest, userData)
	{
	}

	public void AuthToServer(string ticket)
	{
		string value = "unityauthenticationservice";
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["identity"] = value;
		dictionary["appid"] = SteamConstants.GetSteamAppID();
		dictionary["ticket"] = ticket;
		dictionary["app_version"] = Application.version;
		dictionary["os"] = Application.platform.ToString() + " " + SystemInfo.operatingSystem;
		dictionary["steam_id"] = SteamConstants.GetUserSteamID();
		PostRequest(dictionary);
	}
}

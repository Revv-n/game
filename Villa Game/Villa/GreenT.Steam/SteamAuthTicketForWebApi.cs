using System;
using GreenT.Data;
using GreenT.HornyScapes.Steam;
using GreenT.Net;
using Steamworks;
using UniRx;

namespace GreenT.Steam;

public class SteamAuthTicketForWebApi
{
	private readonly User _user;

	private readonly IDataStorage _dataStorage;

	private readonly string _playerIDKey;

	private readonly string playerIDKey;

	private readonly SteamAuthorizationRequestProcessor _steamAuthorization;

	private Subject<User> onSuccess = new Subject<User>();

	private Subject<string> onFail = new Subject<string>();

	public IObservable<User> OnSuccess => onSuccess.AsObservable();

	public IObservable<string> OnFail => onFail.AsObservable();

	public SteamAuthTicketForWebApi(SteamAuthorizationRequestProcessor steamAuthorization, User user, IDataStorage dataStorage, string playerIDKey)
	{
		_steamAuthorization = steamAuthorization;
		_user = user;
		_dataStorage = dataStorage;
		_playerIDKey = playerIDKey;
	}

	public void AuthTicketForWebApiResponse()
	{
		Callback<GetTicketForWebApiResponse_t>.Create(OnAuthCallback);
		SignInWithSteam();
	}

	private void OnAuthCallback(GetTicketForWebApiResponse_t response)
	{
		string sessionTicket = BitConverter.ToString(response.m_rgubTicket).Replace("-", string.Empty);
		ConnectToServer(sessionTicket);
	}

	private void ConnectToServer(string m_SessionTicket)
	{
		_steamAuthorization.AddListener(OnAuth);
		_steamAuthorization.AuthToServer(m_SessionTicket);
	}

	private void OnAuth(Response<UserDataMapper> response)
	{
		_dataStorage.SetString(_playerIDKey, response.Data.PlayerID);
		_user.Init(response.Data);
		_user.SetPlatformId(SteamUser.GetSteamID().ToString());
		onSuccess.OnNext(_user);
	}

	private void SignInWithSteam()
	{
		SteamUser.GetAuthTicketForWebApi("unityauthenticationservice");
	}
}

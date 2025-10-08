using System;
using System.Collections.Generic;
using Erolabs.Sdk.Unity;
using Games.Coresdk.Unity;
using GreenT.Data;
using GreenT.HornyScapes.Net;
using GreenT.Net;
using GreenT.Net.User;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Erolabs;

public class ErolabsSDKAuthorization : IDisposable
{
	private readonly Subject<User> onSuccess = new Subject<User>();

	private readonly Subject<Unit> onAccountBind = new Subject<Unit>();

	private readonly TokenAuthorizationRequestProcessor tokenAuthProcessor;

	private readonly IDataStorage dataStorage;

	private readonly User user;

	private readonly Hyena hyena;

	private Action<Response<UserDataMapper>> onAuthListener;

	private Action<Response<UserDataMapper>> onPostAccountBindGame;

	private Action onBindCallback;

	private readonly CompositeDisposable hyenaCallback = new CompositeDisposable();

	private const string GameId = "156";

	private string erolabsUserId;

	public IObservable<User> OnSuccess => Observable.AsObservable<User>((IObservable<User>)onSuccess);

	public IObservable<Unit> OnAccountBind => Observable.AsObservable<Unit>((IObservable<Unit>)onAccountBind);

	public ErolabsSDKAuthorization(TokenAuthorizationRequestProcessor tokenAuthProcessor, User user, IDataStorage dataStorage, Hyena hyena, GameStarter gameStarter)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		this.tokenAuthProcessor = tokenAuthProcessor;
		this.user = user;
		this.hyena = hyena;
		this.dataStorage = dataStorage;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			HyenaCallback();
		}), (ICollection<IDisposable>)hyenaCallback);
	}

	public void OpenLoginForm()
	{
		if (PlayerPrefs.GetInt(RequestInstaller.DummyAuthKey, 0) == 1)
		{
			PlayerPrefs.SetInt(RequestInstaller.DummyAuthKey, 0);
			DummyAuth();
		}
		else
		{
			ErolabsSDK.OpenLogin("156", delegate(ProfileResult result)
			{
				HandleAuth(result.Data.user_info, isBindFlow: false);
			});
		}
	}

	private void DummyAuth()
	{
		dataStorage.SetString("Player ID", PlayerPrefs.GetString(RequestInstaller.PlayerIdKey, "userid"));
		onSuccess.OnNext(user);
	}

	public void BindAccount(Action onBindCallback = null)
	{
		this.onBindCallback = onBindCallback;
		ErolabsSDK.OpenAccountBindGame("156", user.PlayerID, delegate(BindProfileResult result)
		{
			if (result.IsSuccess)
			{
				HandleAuth(result.Data.user_info, isBindFlow: true);
			}
		});
	}

	private void HandleAuth(ProfileResult.user_info userInfo, bool isBindFlow)
	{
		bool isGuest = string.IsNullOrEmpty(userInfo.account);
		string erolabsNick = userInfo.nickname;
		string token = ErolabsSDK.Token;
		ClearListeners();
		onAuthListener = delegate(Response<UserDataMapper> response)
		{
			CompleteAuthorization(response, userInfo.user_id, isGuest, erolabsNick, isBindFlow);
		};
		tokenAuthProcessor.AddListener(onAuthListener);
		onPostAccountBindGame = PostAccountBindGame;
		tokenAuthProcessor.AddListener(onPostAccountBindGame);
		tokenAuthProcessor.Auth(token);
	}

	private void CompleteAuthorization(Response<UserDataMapper> response, string erolabsUserId, bool isGuest, string erolabsNick, bool isBindFlow)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		user.Init(response.Data, isGuest, erolabsNick);
		this.erolabsUserId = erolabsUserId;
		if (!isBindFlow)
		{
			onSuccess.OnNext(user);
			return;
		}
		onAccountBind.OnNext(Unit.Default);
		onBindCallback?.Invoke();
	}

	private void HyenaCallback()
	{
		if (erolabsUserId != null)
		{
			hyena.Login(user.PlayerID, erolabsUserId);
			hyenaCallback.Dispose();
		}
	}

	private void ClearListeners()
	{
		if (onAuthListener != null)
		{
			tokenAuthProcessor.RemoveListener(onAuthListener);
		}
		if (onPostAccountBindGame != null)
		{
			tokenAuthProcessor.RemoveListener(onPostAccountBindGame);
		}
	}

	private void PostAccountBindGame(Response<UserDataMapper> response)
	{
		if (!user.IsGuest)
		{
			ErolabsSDK.PostAccountBindGame("156", response.Data.PlayerID, OnPostAccountBindGame);
		}
	}

	private void OnPostAccountBindGame(AccountBindGameResult result)
	{
	}

	public void Dispose()
	{
		tokenAuthProcessor?.Dispose();
	}
}

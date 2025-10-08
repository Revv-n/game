using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Exceptions;
using GreenT.HornyScapes.Monetization.Windows.Steam;
using GreenT.Steam.Achievements;
using UniRx;

namespace GreenT.Steam;

public class SteamBridge : ISteamBridge, IDisposable
{
	private readonly SteamSDKService _steamSDKService;

	private readonly SteamWarningMessageHandler _steamWarningMessageHandler;

	private readonly SteamPaymentAuthorization _steamPaymentAuthorization;

	private readonly SteamAuthTicketForWebApi _steamAuthTicketForWebApi;

	private readonly AchievementEntryPoint _achievementEntryPoint;

	private readonly RegionPriceResolver _regionPriceResolver;

	private readonly IExceptionHandler _exceptionHandler;

	private CompositeDisposable _connectStream = new CompositeDisposable();

	public SteamBridge(SteamSDKService steamSDKService, SteamWarningMessageHandler steamWarningMessageHandler, SteamPaymentAuthorization steamPaymentAuthorization, SteamAuthTicketForWebApi steamAuthTicketForWebApi, IExceptionHandler exceptionHandler, AchievementEntryPoint achievementEntryPoint, RegionPriceResolver regionPriceResolver)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		_steamSDKService = steamSDKService;
		_steamWarningMessageHandler = steamWarningMessageHandler;
		_steamPaymentAuthorization = steamPaymentAuthorization;
		_steamAuthTicketForWebApi = steamAuthTicketForWebApi;
		_exceptionHandler = exceptionHandler;
		_achievementEntryPoint = achievementEntryPoint;
		_regionPriceResolver = regionPriceResolver;
	}

	public void InitAuth(Action<User> onSuccess)
	{
		_steamSDKService.Initialize();
		ConnectAccountWithServer(onSuccess);
		_steamWarningMessageHandler.SetWarningMessageHook();
	}

	private void ConnectAccountWithServer(Action<User> onSuccess)
	{
		_connectStream.Clear();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<User>(_steamAuthTicketForWebApi.OnSuccess, (Action<User>)delegate(User _user)
		{
			OnAuth(_user, onSuccess);
		}), (ICollection<IDisposable>)_connectStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(_steamAuthTicketForWebApi.OnFail, (Action<string>)_exceptionHandler.Handle), (ICollection<IDisposable>)_connectStream);
		_steamPaymentAuthorization.PaymentTransactionAuth();
		Authorize();
	}

	public void OnAuth(User user, Action<User> onSuccess)
	{
		_achievementEntryPoint.Initialize();
		_regionPriceResolver.OnAuth();
		SteamDataSuite.Initialize();
		OnSuccess(user, onSuccess);
	}

	public void OnSuccess(User user, Action<User> onSuccess)
	{
		onSuccess(user);
	}

	public void Dispose()
	{
		CompositeDisposable connectStream = _connectStream;
		if (connectStream != null)
		{
			connectStream.Dispose();
		}
	}

	private void Authorize()
	{
		_steamAuthTicketForWebApi.AuthTicketForWebApiResponse();
	}
}

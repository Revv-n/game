using System;
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
		_steamAuthTicketForWebApi.OnSuccess.Subscribe(delegate(User _user)
		{
			OnAuth(_user, onSuccess);
		}).AddTo(_connectStream);
		_steamAuthTicketForWebApi.OnFail.Subscribe(_exceptionHandler.Handle).AddTo(_connectStream);
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
		_connectStream?.Dispose();
	}

	private void Authorize()
	{
		_steamAuthTicketForWebApi.AuthTicketForWebApiResponse();
	}
}

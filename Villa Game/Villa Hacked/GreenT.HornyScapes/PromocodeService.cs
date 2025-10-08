using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Content;
using GreenT.Types;
using StripClub.Model;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class PromocodeService
{
	private PromocodeActivationRequest promocodeActivationRequest;

	private CurrencyContentFactory currencyContentFactory;

	private ContentAdder contentAdder;

	private PromocodePopupWindowOpener promocodePopupWindowOpener;

	private PromocodePopupWindow popupWindow;

	private readonly Subject<string> responseMessageSubject = new Subject<string>();

	private IDisposable promocodeActivationDisposable;

	private IDisposable hidePromocodeWindowStream;

	private IDisposable showRewardStream;

	private IDisposable wait;

	private const string key = "ui.settings.promo.msg";

	private const string msg_alreadyActivated_key = "ui.settings.promo.msg.activated";

	private const string msg_wrong_key = "ui.settings.promo.msg.wrong";

	private const string msg_expired_key = "ui.settings.promo.msg.expired";

	private string msg_key = string.Empty;

	public IObservable<string> OnResponseMessage => (IObservable<string>)responseMessageSubject;

	[Inject]
	public void Init(PromocodeActivationRequest promocodeActivationRequest, CurrencyContentFactory currencyContentFactory, ContentAdder contentAdder, PromocodePopupWindowOpener promocodePopupWindowOpener)
	{
		this.promocodeActivationRequest = promocodeActivationRequest;
		this.currencyContentFactory = currencyContentFactory;
		this.contentAdder = contentAdder;
		this.promocodePopupWindowOpener = promocodePopupWindowOpener;
	}

	public void TryToActivatePromocode(string promocodeName)
	{
		ShowWaitingResultWindow();
		wait?.Dispose();
		wait = ObservableExtensions.Subscribe<long>(Observable.Take<long>(Observable.Where<long>(Observable.SkipUntil<long, long>(Observable.EveryUpdate(), Observable.Timer(TimeSpan.FromSeconds(0.5))), (Func<long, bool>)((long _) => popupWindow.IsOpened)), 1), (Action<long>)delegate
		{
			promocodeActivationDisposable?.Dispose();
			promocodeActivationDisposable = ObservableExtensions.Subscribe<PromocodeRewardResponse>(ActivatePromocode(promocodeName), (Action<PromocodeRewardResponse>)PromocodeResponse);
			HideWaitingResultWindow(forcedClose: true, 3f);
		});
	}

	public IObservable<PromocodeRewardResponse> ActivatePromocode(string promocodeName)
	{
		string platformName = PlatformHelper.GetPlatformName();
		return Observable.Catch<PromocodeRewardResponse, Exception>(promocodeActivationRequest.Post(promocodeName, platformName), (Func<Exception, IObservable<PromocodeRewardResponse>>)delegate(Exception ex)
		{
			msg_key = string.Empty;
			if (ex.Message.Contains("ResponseCode = 404") || ex.Message.Contains("ResponseCode = 512"))
			{
				msg_key = "ui.settings.promo.msg.wrong";
			}
			if (ex.Message.Contains("ResponseCode = 409") || ex.Message.Contains("ResponseCode = 511"))
			{
				msg_key = "ui.settings.promo.msg.activated";
			}
			if (ex.Message.Contains("ResponseCode = 513"))
			{
				msg_key = "ui.settings.promo.msg.expired";
			}
			responseMessageSubject.OnNext(msg_key);
			HideWaitingResultWindow();
			Debug.LogError("ERROR WHEN TRYING TO ACTIVATE PROMOCODE: " + promocodeName + " ON PLATFORM: " + platformName);
			return Observable.Empty<PromocodeRewardResponse>();
		});
	}

	private void PromocodeResponse(PromocodeRewardResponse response)
	{
		responseMessageSubject.OnNext("");
		HideWaitingResultWindow();
		ShowReward(response);
	}

	private void ShowReward(PromocodeRewardResponse response)
	{
		showRewardStream = ObservableExtensions.Subscribe<long>(Observable.Take<long>(Observable.SkipUntil<long, long>(Observable.EveryUpdate(), Observable.Timer(TimeSpan.FromSeconds(0.5))), 1), (Action<long>)delegate
		{
			GetRewardForPromocode(response.Reward.SoftCurrency, response.Reward.HardCurrency);
		});
	}

	private void GetRewardForPromocode(int soft_reward, int hard_reward)
	{
		CurrencyLinkedContent currencyLinkedContent = currencyContentFactory.Create(soft_reward, CurrencyType.Soft, new LinkedContentAnalyticData(CurrencyAmplitudeAnalytic.SourceType.Promocode), default(CompositeIdentificator));
		CurrencyLinkedContent content = currencyContentFactory.Create(hard_reward, CurrencyType.Hard, new LinkedContentAnalyticData(CurrencyAmplitudeAnalytic.SourceType.Promocode), default(CompositeIdentificator));
		currencyLinkedContent.Insert(content);
		contentAdder.AddContent(currencyLinkedContent);
	}

	private void ShowWaitingResultWindow()
	{
		popupWindow = promocodePopupWindowOpener.OpenAdditiveWindow() as PromocodePopupWindow;
	}

	private void HideWaitingResultWindow(bool forcedClose = false, float waitTime = 0f)
	{
		hidePromocodeWindowStream = ObservableExtensions.Subscribe<long>(Observable.Take<long>(Observable.Where<long>(Observable.SkipUntil<long, long>(Observable.EveryUpdate(), Observable.Timer(TimeSpan.FromSeconds(waitTime))), (Func<long, bool>)((long _) => popupWindow.IsOpened || forcedClose)), 1), (Action<long>)delegate
		{
			popupWindow?.Close();
		});
	}

	private void Dispose()
	{
		hidePromocodeWindowStream?.Dispose();
		showRewardStream?.Dispose();
		promocodeActivationDisposable?.Dispose();
		wait?.Dispose();
	}
}

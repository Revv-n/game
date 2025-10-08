using System;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;

namespace GreenT.HornyScapes.Subscription.Push;

public class SubscriptionTimePushNotifier : IDisposable
{
	public readonly GenericTimer Timer = new GenericTimer();

	private readonly IClock _clock;

	private readonly SubscriptionPushSettings _settings;

	private readonly SubscriptionStorage _subscriptionStorage;

	private IDisposable _timerStartStream;

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public SubscriptionTimePushNotifier(SubscriptionPushSettings settings, SubscriptionStorage subscriptionStorage, IClock clock)
	{
		_clock = clock;
		_settings = settings;
		_subscriptionStorage = subscriptionStorage;
	}

	public void Set()
	{
		_settings.OnPush().Subscribe(delegate
		{
			SetupTimer();
		}).AddTo(_compositeDisposable);
	}

	private void StartTimer(TimeSpan pushTime)
	{
		Timer.Start(pushTime);
	}

	private void SetupTimer()
	{
		if (!_settings.PushOffset.HasValue)
		{
			StartTimer(TimeSpan.FromSeconds(_settings.PushTime));
		}
		else
		{
			HandleProlongOffer();
		}
	}

	private void HandleProlongOffer()
	{
		SubscriptionModel offerSubscription = GetOfferSubscription();
		IObservable<SubscriptionModel> source = ((offerSubscription != null) ? Observable.Return(offerSubscription) : _subscriptionStorage.OnNew.Where((SubscriptionModel item) => _settings.SubscriptionID.Any((SubscriptionLot sub) => sub.ID == item.BaseID)));
		_timerStartStream?.Dispose();
		_timerStartStream = source.Select(GetTimeToPush).Subscribe(delegate(TimeSpan span)
		{
			StartTimer((span.Ticks <= 0) ? TimeSpan.FromSeconds(_settings.PushTime) : span);
		});
	}

	private SubscriptionModel GetOfferSubscription()
	{
		return _subscriptionStorage.Collection.FirstOrDefault((SubscriptionModel item) => _settings.SubscriptionID.Any((SubscriptionLot sub) => sub.ExtensionID == item.BaseID));
	}

	private TimeSpan GetTimeToPush(SubscriptionModel model)
	{
		if (!_settings.PushOffset.HasValue)
		{
			return TimeSpan.Zero;
		}
		DateTime time = _clock.GetTime();
		return time + model.Duration.TimeLeft - TimeSpan.FromSeconds(_settings.PushOffset.Value) - time;
	}

	public IObservable<Unit> OnPushRequest()
	{
		if (Timer.InitTime == TimeSpan.Zero)
		{
			if (!_settings.PushOffset.HasValue)
			{
				return Observable.Return(default(Unit));
			}
			SubscriptionModel offerSubscription = GetOfferSubscription();
			TimeSpan timeToPush = GetTimeToPush(offerSubscription);
			if (timeToPush.Ticks > 0)
			{
				return Observable.EveryUpdate().Delay(timeToPush).AsUnitObservable();
			}
			return Observable.Return(default(Unit));
		}
		return Timer.OnTimeIsUp.AsUnitObservable();
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}
}

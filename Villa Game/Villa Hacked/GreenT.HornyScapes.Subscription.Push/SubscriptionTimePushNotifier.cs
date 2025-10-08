using System;
using System.Collections.Generic;
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		_clock = clock;
		_settings = settings;
		_subscriptionStorage = subscriptionStorage;
	}

	public void Set()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(_settings.OnPush(), (Action<Unit>)delegate
		{
			SetupTimer();
		}), (ICollection<IDisposable>)_compositeDisposable);
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
		IObservable<SubscriptionModel> observable = ((offerSubscription != null) ? Observable.Return<SubscriptionModel>(offerSubscription) : Observable.Where<SubscriptionModel>(_subscriptionStorage.OnNew, (Func<SubscriptionModel, bool>)((SubscriptionModel item) => _settings.SubscriptionID.Any((SubscriptionLot sub) => sub.ID == item.BaseID))));
		_timerStartStream?.Dispose();
		_timerStartStream = ObservableExtensions.Subscribe<TimeSpan>(Observable.Select<SubscriptionModel, TimeSpan>(observable, (Func<SubscriptionModel, TimeSpan>)GetTimeToPush), (Action<TimeSpan>)delegate(TimeSpan span)
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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
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
				return Observable.AsUnitObservable<long>(Observable.Delay<long>(Observable.EveryUpdate(), timeToPush));
			}
			return Observable.Return(default(Unit));
		}
		return Observable.AsUnitObservable<GenericTimer>(Timer.OnTimeIsUp);
	}

	public void Dispose()
	{
		CompositeDisposable compositeDisposable = _compositeDisposable;
		if (compositeDisposable != null)
		{
			compositeDisposable.Dispose();
		}
	}
}

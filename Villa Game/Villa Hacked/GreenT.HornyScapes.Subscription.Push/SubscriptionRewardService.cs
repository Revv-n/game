using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.UI;
using ModestTree;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Subscription.Push;

public class SubscriptionRewardService : IInitializable, IDisposable
{
	private readonly LootboxOpener _lootboxOpener;

	private readonly MainScreenIndicator _screenIndicator;

	private readonly SubscriptionStorage _subscriptionStorage;

	private readonly SubscriptionPushNotifierFactory _notifierFactory;

	private readonly LotManager _lotManager;

	private readonly ReactiveCollection<SubscriptionDailyPushNotifier> _notifiers = new ReactiveCollection<SubscriptionDailyPushNotifier>();

	private readonly ReactiveCollection<SubscriptionDailyPushNotifier> _pushQueue = new ReactiveCollection<SubscriptionDailyPushNotifier>();

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public SubscriptionRewardService(SubscriptionStorage subscriptionStorage, SubscriptionPushNotifierFactory notifierFactory, LotManager lotManager, LootboxOpener lootboxOpener, MainScreenIndicator screenIndicator)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		_subscriptionStorage = subscriptionStorage;
		_notifierFactory = notifierFactory;
		_lotManager = lotManager;
		_lootboxOpener = lootboxOpener;
		_screenIndicator = screenIndicator;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionDailyPushNotifier>(Observable.SelectMany<SubscriptionDailyPushNotifier, SubscriptionDailyPushNotifier>(Observable.Select<CollectionAddEvent<SubscriptionDailyPushNotifier>, SubscriptionDailyPushNotifier>(_notifiers.ObserveAdd(), (Func<CollectionAddEvent<SubscriptionDailyPushNotifier>, SubscriptionDailyPushNotifier>)((CollectionAddEvent<SubscriptionDailyPushNotifier> addEvent) => addEvent.Value)), (Func<SubscriptionDailyPushNotifier, IObservable<SubscriptionDailyPushNotifier>>)((SubscriptionDailyPushNotifier notifier) => WaitForPush(notifier, force: true))), (Action<SubscriptionDailyPushNotifier>)delegate(SubscriptionDailyPushNotifier notifier)
		{
			((Collection<SubscriptionDailyPushNotifier>)(object)_pushQueue).Add(notifier);
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionDailyPushNotifier>(Observable.SelectMany<SubscriptionDailyPushNotifier, SubscriptionDailyPushNotifier>(Observable.Select<CollectionAddEvent<SubscriptionDailyPushNotifier>, SubscriptionDailyPushNotifier>(_pushQueue.ObserveAdd(), (Func<CollectionAddEvent<SubscriptionDailyPushNotifier>, SubscriptionDailyPushNotifier>)((CollectionAddEvent<SubscriptionDailyPushNotifier> addEvent) => addEvent.Value)), (Func<SubscriptionDailyPushNotifier, IObservable<SubscriptionDailyPushNotifier>>)HandleDailyLifecycle), (Action<SubscriptionDailyPushNotifier>)HandlePushedNotifier), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionDailyPushNotifier>(Observable.SelectMany<SubscriptionDailyPushNotifier, SubscriptionDailyPushNotifier>(Observable.Select<CollectionRemoveEvent<SubscriptionDailyPushNotifier>, SubscriptionDailyPushNotifier>(_pushQueue.ObserveRemove(), (Func<CollectionRemoveEvent<SubscriptionDailyPushNotifier>, SubscriptionDailyPushNotifier>)((CollectionRemoveEvent<SubscriptionDailyPushNotifier> removeEvent) => removeEvent.Value)), (Func<SubscriptionDailyPushNotifier, IObservable<SubscriptionDailyPushNotifier>>)((SubscriptionDailyPushNotifier notifier) => WaitForPush(notifier))), (Action<SubscriptionDailyPushNotifier>)delegate(SubscriptionDailyPushNotifier notifier)
		{
			((Collection<SubscriptionDailyPushNotifier>)(object)_pushQueue).Add(notifier);
		}), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionDailyPushNotifier>(Observable.Do<SubscriptionDailyPushNotifier>(Observable.Select<SubscriptionModel, SubscriptionDailyPushNotifier>(Observable.Where<SubscriptionModel>(_subscriptionStorage.OnNew, (Func<SubscriptionModel, bool>)HasDaily), (Func<SubscriptionModel, SubscriptionDailyPushNotifier>)((SubscriptionModel model) => _notifierFactory.Create(model))), (Action<SubscriptionDailyPushNotifier>)delegate(SubscriptionDailyPushNotifier notifier)
		{
			notifier.Set();
		}), (Action<SubscriptionDailyPushNotifier>)delegate(SubscriptionDailyPushNotifier notifier)
		{
			((Collection<SubscriptionDailyPushNotifier>)(object)_notifiers).Add(notifier);
		}), (ICollection<IDisposable>)_compositeDisposable);
	}

	private bool HasDaily(SubscriptionModel model)
	{
		return _lotManager.GetLot<SubscriptionLot>().FirstOrDefault((SubscriptionLot lot) => lot.ID == model.BaseID)?.RechargeReward != null;
	}

	private IObservable<SubscriptionDailyPushNotifier> WaitForPush(SubscriptionDailyPushNotifier notifier, bool force = false)
	{
		return Observable.Take<SubscriptionDailyPushNotifier>(Observable.Select<Unit, SubscriptionDailyPushNotifier>(Observable.Where<Unit>(notifier.OnPushRequest(force), (Func<Unit, bool>)((Unit _) => _subscriptionStorage.Collection.Any((SubscriptionModel model) => notifier.Model == model))), (Func<Unit, SubscriptionDailyPushNotifier>)((Unit _) => notifier)), 1);
	}

	private IObservable<SubscriptionDailyPushNotifier> HandleDailyLifecycle(SubscriptionDailyPushNotifier notifier)
	{
		if (LinqExtensions.IsEmpty<SubscriptionDailyPushNotifier>(LinqExtensions.Except<SubscriptionDailyPushNotifier>((IEnumerable<SubscriptionDailyPushNotifier>)_pushQueue, notifier)))
		{
			return Observable.Take<SubscriptionDailyPushNotifier>(Observable.SelectMany<SubscriptionDailyPushNotifier, SubscriptionDailyPushNotifier>(Observable.Select<Unit, SubscriptionDailyPushNotifier>(Observable.Take<Unit>(WaitForMeta(), 1), (Func<Unit, SubscriptionDailyPushNotifier>)((Unit _) => Push(notifier))), (Func<SubscriptionDailyPushNotifier, IObservable<SubscriptionDailyPushNotifier>>)WaitForLootboxOpen), 1);
		}
		return Observable.Take<SubscriptionDailyPushNotifier>(Observable.SelectMany<SubscriptionDailyPushNotifier, SubscriptionDailyPushNotifier>(Observable.Select<Unit, SubscriptionDailyPushNotifier>(Observable.SelectMany<CollectionRemoveEvent<SubscriptionDailyPushNotifier>, Unit>(_pushQueue.ObserveRemove(), (Func<CollectionRemoveEvent<SubscriptionDailyPushNotifier>, IObservable<Unit>>)((CollectionRemoveEvent<SubscriptionDailyPushNotifier> _) => Observable.Take<Unit>(WaitForMeta(), 1))), (Func<Unit, SubscriptionDailyPushNotifier>)((Unit _) => Push(notifier))), (Func<SubscriptionDailyPushNotifier, IObservable<SubscriptionDailyPushNotifier>>)WaitForLootboxOpen), 1);
	}

	private void HandlePushedNotifier(SubscriptionDailyPushNotifier notifier)
	{
		notifier.SetClaimed();
		((Collection<SubscriptionDailyPushNotifier>)(object)_pushQueue).Remove(notifier);
	}

	private SubscriptionDailyPushNotifier Push(SubscriptionDailyPushNotifier notifier)
	{
		LootboxLinkedContent lootboxLinkedContent = (LootboxLinkedContent)notifier.Lot.RechargeReward;
		if (lootboxLinkedContent == null)
		{
			return null;
		}
		_lootboxOpener.OpenSubscriptionDaily(lootboxLinkedContent.Lootbox, CurrencyAmplitudeAnalytic.SourceType.None, notifier.Model);
		return notifier;
	}

	private IObservable<SubscriptionDailyPushNotifier> WaitForLootboxOpen(SubscriptionDailyPushNotifier notifier)
	{
		return Observable.Select<Lootbox, SubscriptionDailyPushNotifier>(Observable.Where<Lootbox>(_lootboxOpener.OnOpen, (Func<Lootbox, bool>)((Lootbox lootbox) => notifier.Lot.RechargeReward is LootboxLinkedContent lootboxLinkedContent && lootbox.ID == lootboxLinkedContent.Lootbox.ID)), (Func<Lootbox, SubscriptionDailyPushNotifier>)((Lootbox _) => notifier));
	}

	private IObservable<Unit> WaitForMeta()
	{
		return Observable.AsUnitObservable<bool>(Observable.Where<bool>((IObservable<bool>)_screenIndicator.IsVisible, (Func<bool, bool>)((bool visible) => visible && ((Collection<SubscriptionDailyPushNotifier>)(object)_pushQueue).Count > 0)));
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

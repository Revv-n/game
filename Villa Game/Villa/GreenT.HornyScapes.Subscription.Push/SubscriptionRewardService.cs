using System;
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
		_subscriptionStorage = subscriptionStorage;
		_notifierFactory = notifierFactory;
		_lotManager = lotManager;
		_lootboxOpener = lootboxOpener;
		_screenIndicator = screenIndicator;
	}

	public void Initialize()
	{
		(from addEvent in _notifiers.ObserveAdd()
			select addEvent.Value).SelectMany((SubscriptionDailyPushNotifier notifier) => WaitForPush(notifier, force: true)).Subscribe(delegate(SubscriptionDailyPushNotifier notifier)
		{
			_pushQueue.Add(notifier);
		}).AddTo(_compositeDisposable);
		(from addEvent in _pushQueue.ObserveAdd()
			select addEvent.Value).SelectMany((Func<SubscriptionDailyPushNotifier, IObservable<SubscriptionDailyPushNotifier>>)HandleDailyLifecycle).Subscribe(HandlePushedNotifier).AddTo(_compositeDisposable);
		(from removeEvent in _pushQueue.ObserveRemove()
			select removeEvent.Value).SelectMany((SubscriptionDailyPushNotifier notifier) => WaitForPush(notifier)).Subscribe(delegate(SubscriptionDailyPushNotifier notifier)
		{
			_pushQueue.Add(notifier);
		}).AddTo(_compositeDisposable);
		(from model in _subscriptionStorage.OnNew.Where(HasDaily)
			select _notifierFactory.Create(model)).Do(delegate(SubscriptionDailyPushNotifier notifier)
		{
			notifier.Set();
		}).Subscribe(delegate(SubscriptionDailyPushNotifier notifier)
		{
			_notifiers.Add(notifier);
		}).AddTo(_compositeDisposable);
	}

	private bool HasDaily(SubscriptionModel model)
	{
		return _lotManager.GetLot<SubscriptionLot>().FirstOrDefault((SubscriptionLot lot) => lot.ID == model.BaseID)?.RechargeReward != null;
	}

	private IObservable<SubscriptionDailyPushNotifier> WaitForPush(SubscriptionDailyPushNotifier notifier, bool force = false)
	{
		return (from _ in notifier.OnPushRequest(force)
			where _subscriptionStorage.Collection.Any((SubscriptionModel model) => notifier.Model == model)
			select notifier).Take(1);
	}

	private IObservable<SubscriptionDailyPushNotifier> HandleDailyLifecycle(SubscriptionDailyPushNotifier notifier)
	{
		if (_pushQueue.Except(notifier).IsEmpty())
		{
			return (from _ in WaitForMeta().Take(1)
				select Push(notifier)).SelectMany((Func<SubscriptionDailyPushNotifier, IObservable<SubscriptionDailyPushNotifier>>)WaitForLootboxOpen).Take(1);
		}
		return (from _ in _pushQueue.ObserveRemove().SelectMany((CollectionRemoveEvent<SubscriptionDailyPushNotifier> _) => WaitForMeta().Take(1))
			select Push(notifier)).SelectMany((Func<SubscriptionDailyPushNotifier, IObservable<SubscriptionDailyPushNotifier>>)WaitForLootboxOpen).Take(1);
	}

	private void HandlePushedNotifier(SubscriptionDailyPushNotifier notifier)
	{
		notifier.SetClaimed();
		_pushQueue.Remove(notifier);
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
		return from lootbox in _lootboxOpener.OnOpen
			where notifier.Lot.RechargeReward is LootboxLinkedContent lootboxLinkedContent && lootbox.ID == lootboxLinkedContent.Lootbox.ID
			select lootbox into _
			select notifier;
	}

	private IObservable<Unit> WaitForMeta()
	{
		return _screenIndicator.IsVisible.Where((bool visible) => visible && _pushQueue.Count > 0).AsUnitObservable();
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}
}

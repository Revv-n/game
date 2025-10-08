using System;
using System.Linq;
using GreenT.HornyScapes.Bank.Offer.UI;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Subscription.Push;

public class SubscriptionOfferPushService : IInitializable, IDisposable
{
	private readonly WindowOpener _sectionWindowOpener;

	private readonly SubscriptionOfferSectionController _offerSectionController;

	private readonly IWindowsManager _windowsManager;

	private readonly MainScreenIndicator _mainScreenIndicator;

	private readonly SubscriptionService _subscriptionService;

	private readonly WindowID _offerWindowID;

	private readonly WindowID _subscriptionOfferWindowID;

	private readonly SubscriptionPushNotifierFactory _notifierFactory;

	private readonly SubscriptionPushSettings.Manager _subscriptionSettingsManager;

	private readonly ReactiveCollection<SubscriptionPushSettings> _pushQueue = new ReactiveCollection<SubscriptionPushSettings>();

	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public SubscriptionOfferPushService(SubscriptionService subscriptionService, WindowID offerWindowID, WindowID subscriptionOfferWindowID, SubscriptionOfferSectionController offerSectionController, IWindowsManager windowsManager, MainScreenIndicator mainScreenIndicator, WindowOpener windowOpener, SubscriptionPushSettings.Manager subscriptionSettingsManager, SubscriptionPushNotifierFactory notifierFactory)
	{
		_offerWindowID = offerWindowID;
		_windowsManager = windowsManager;
		_notifierFactory = notifierFactory;
		_sectionWindowOpener = windowOpener;
		_mainScreenIndicator = mainScreenIndicator;
		_subscriptionService = subscriptionService;
		_offerSectionController = offerSectionController;
		_subscriptionOfferWindowID = subscriptionOfferWindowID;
		_subscriptionSettingsManager = subscriptionSettingsManager;
	}

	public void Initialize()
	{
		_subscriptionService.InitializeSubject.AsObservable().SelectMany((bool _) => _subscriptionSettingsManager.Collection).Subscribe(SetupNotifier)
			.AddTo(_compositeDisposable);
		(from count in _pushQueue.ObserveCountChanged()
			where count > 0
			select count).SelectMany((int _) => WaitForMeta()).Subscribe(Push).AddTo(_compositeDisposable);
	}

	private void Push(SubscriptionPushSettings settings)
	{
		_sectionWindowOpener.Click();
		_offerSectionController.LoadSection(settings);
		settings.InvokeOnPushedEvent();
		_pushQueue.RemoveAt(0);
	}

	private IObservable<SubscriptionPushSettings> WaitForMeta()
	{
		return from _ in _mainScreenIndicator.IsVisible.Where((bool visible) => visible && _pushQueue.Count > 0 && !_windowsManager.GetWindow(_offerWindowID).IsOpened && !_windowsManager.GetWindow(_subscriptionOfferWindowID).IsOpened).Take(1)
			select _pushQueue.First();
	}

	private void SetupNotifier(SubscriptionPushSettings settings)
	{
		(from status in settings.Lock.IsOpen
			where status
			select status into _
			select _notifierFactory.Create(settings)).Do(delegate(SubscriptionTimePushNotifier notifier)
		{
			notifier.Set();
		}).SelectMany((SubscriptionTimePushNotifier notifier) => from _ in notifier.OnPushRequest()
			where settings.Lock.IsOpen.Value
			select _).Subscribe(delegate
		{
			_pushQueue.Add(settings);
		})
			.AddTo(_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}
}

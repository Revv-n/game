using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionPushSettings>(Observable.SelectMany<bool, SubscriptionPushSettings>(Observable.AsObservable<bool>((IObservable<bool>)_subscriptionService.InitializeSubject), (Func<bool, IEnumerable<SubscriptionPushSettings>>)((bool _) => _subscriptionSettingsManager.Collection)), (Action<SubscriptionPushSettings>)SetupNotifier), (ICollection<IDisposable>)_compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SubscriptionPushSettings>(Observable.SelectMany<int, SubscriptionPushSettings>(Observable.Where<int>(_pushQueue.ObserveCountChanged(false), (Func<int, bool>)((int count) => count > 0)), (Func<int, IObservable<SubscriptionPushSettings>>)((int _) => WaitForMeta())), (Action<SubscriptionPushSettings>)Push), (ICollection<IDisposable>)_compositeDisposable);
	}

	private void Push(SubscriptionPushSettings settings)
	{
		_sectionWindowOpener.Click();
		_offerSectionController.LoadSection(settings);
		settings.InvokeOnPushedEvent();
		((Collection<SubscriptionPushSettings>)(object)_pushQueue).RemoveAt(0);
	}

	private IObservable<SubscriptionPushSettings> WaitForMeta()
	{
		return Observable.Select<bool, SubscriptionPushSettings>(Observable.Take<bool>(Observable.Where<bool>((IObservable<bool>)_mainScreenIndicator.IsVisible, (Func<bool, bool>)((bool visible) => visible && ((Collection<SubscriptionPushSettings>)(object)_pushQueue).Count > 0 && !_windowsManager.GetWindow(_offerWindowID).IsOpened && !_windowsManager.GetWindow(_subscriptionOfferWindowID).IsOpened)), 1), (Func<bool, SubscriptionPushSettings>)((bool _) => ((IEnumerable<SubscriptionPushSettings>)_pushQueue).First()));
	}

	private void SetupNotifier(SubscriptionPushSettings settings)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.SelectMany<SubscriptionTimePushNotifier, Unit>(Observable.Do<SubscriptionTimePushNotifier>(Observable.Select<bool, SubscriptionTimePushNotifier>(Observable.Where<bool>((IObservable<bool>)settings.Lock.IsOpen, (Func<bool, bool>)((bool status) => status)), (Func<bool, SubscriptionTimePushNotifier>)((bool _) => _notifierFactory.Create(settings))), (Action<SubscriptionTimePushNotifier>)delegate(SubscriptionTimePushNotifier notifier)
		{
			notifier.Set();
		}), (Func<SubscriptionTimePushNotifier, IObservable<Unit>>)((SubscriptionTimePushNotifier notifier) => Observable.Where<Unit>(notifier.OnPushRequest(), (Func<Unit, bool>)((Unit _) => settings.Lock.IsOpen.Value)))), (Action<Unit>)delegate
		{
			((Collection<SubscriptionPushSettings>)(object)_pushQueue).Add(settings);
		}), (ICollection<IDisposable>)_compositeDisposable);
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

using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using GreenT.HornyScapes.UI;
using GreenT.Types;
using GreenT.UI;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public class PushController : MonoView<OfferSettings.Manager>
{
	[SerializeField]
	private WindowOpener windowOpener;

	[SerializeField]
	private ContentType contentType;

	private ReactiveCollection<OfferSettings> pushCollection = new ReactiveCollection<OfferSettings>();

	private SectionController sectionController;

	private IWindowsManager windowsManager;

	private ScreenIndicator screenIndicator;

	private CompositeDisposable disposables = new CompositeDisposable();

	private Subject<OfferSettings> currentShownOffer = new Subject<OfferSettings>();

	private CharacterProvider _characterProvider;

	private SkinDataLoadingController _skinDataLoadingController;

	public ContentType ContentType => contentType;

	[Inject]
	private void Init(SectionController sectionController, IWindowsManager windowsManager, ScreenIndicator screenIndicator, IDictionary<ContentType, OfferSettings.Manager> managerCluster, CharacterProvider characterProvider, SkinDataLoadingController skinDataLoadingController)
	{
		_skinDataLoadingController = skinDataLoadingController;
		this.sectionController = sectionController;
		this.windowsManager = windowsManager;
		this.screenIndicator = screenIndicator;
		OfferSettings.Manager source = managerCluster[contentType];
		base.Set(source);
		_characterProvider = characterProvider;
	}

	public override void Set(OfferSettings.Manager source)
	{
		base.Set(source);
		pushCollection.Clear();
		if (base.isActiveAndEnabled)
		{
			disposables?.Clear();
			windowsManager.OnCloseWindow -= RestartPushTimerForJustShownOffer;
			OnEnable();
		}
	}

	private void OnEnable()
	{
		TrackOffersToBePushed();
		TrackOffersToRemoveFromPushQueue();
		PushOffers();
		windowsManager.OnCloseWindow += RestartPushTimerForJustShownOffer;
	}

	private void RestartPushTimerForJustShownOffer(IWindow window)
	{
		if (window is OfferWindow && sectionController.Source != null)
		{
			OfferSettings source = sectionController.Source;
			if (base.Source.Collection.Contains(source))
			{
				currentShownOffer.OnNext(source);
				pushCollection.Remove(source);
				RestartTrackOnClose(source);
			}
		}
	}

	private void AddOfferToCollectionForPush(OfferSettings offer)
	{
		if (!pushCollection.Contains(offer))
		{
			IObservable<LootboxLinkedContent> source = offer.Bundles.Select((BundleLot _bundle) => _bundle.Content).OfType<LootboxLinkedContent>().ToObservable();
			IObservable<Unit> observable = (from _girlIds in source.SelectMany((LootboxLinkedContent content) => content.Lootbox.CharacterIdPossibleDrops)
				select _characterProvider.Get(_girlIds)).Concat().DefaultIfEmpty().AsUnitObservable();
			IObservable<Unit> observable2 = (from ids in source.SelectMany((LootboxLinkedContent content) => content.Lootbox.SkinIdPossibleDrops)
				select _skinDataLoadingController.InsertDataOnLoad(ids)).Concat().DefaultIfEmpty().AsUnitObservable();
			(from _ in Observable.WhenAll(observable, observable2)
				select offer).Subscribe(delegate
			{
				pushCollection.Add(offer);
			});
		}
	}

	private void ShowNext()
	{
		OfferSettings offerSettings = pushCollection[0];
		pushCollection.RemoveAt(0);
		windowOpener.Click();
		sectionController.LoadSection(offerSettings);
		offerSettings.InvokeOnPushedEvent();
	}

	private void RestartTrackOnClose(OfferSettings offer)
	{
		EmitUnlockedOfferByTimer(offer).Subscribe(AddOfferToCollectionForPush).AddTo(disposables);
	}

	private IObservable<OfferSettings> EmitUnlockedOfferByTimer(OfferSettings offerSettings)
	{
		return from _ in offerSettings.LockWithTimer.IsOpen.FirstOrDefault((bool x) => x).ContinueWith(Observable.Timer(offerSettings.PushTime, Scheduler.MainThreadIgnoreTimeScale)).TakeUntil(ObservableOfferLockout(offerSettings))
				.TakeUntil(currentShownOffer.FirstOrDefault((OfferSettings _offer) => _offer != null && _offer.ID == offerSettings.ID))
			where offerSettings.Bundles.All((BundleLot _bundle) => _bundle.IsAvailable())
			select offerSettings;
	}

	private IObservable<bool> ObservableOfferLockout(OfferSettings offerSettings)
	{
		return offerSettings.LockWithTimer.IsOpen.Skip(1).First((bool x) => !x);
	}

	private void PushOffers()
	{
		(from x in pushCollection.ObserveAdd().CombineLatest(screenIndicator.IsVisible, (CollectionAddEvent<OfferSettings> _addEvent, bool _isScreenVisible) => _isScreenVisible && pushCollection.Any())
			where x
			select x).Subscribe(delegate
		{
			ShowNext();
		}).AddTo(disposables);
	}

	private void TrackOffersToBePushed()
	{
		TimeSpan dueTime = TimeSpan.FromSeconds(3.0);
		IConnectableObservable<OfferSettings> connectableObservable = base.Source.Collection.ToObservable().Merge(base.Source.OnNew).Publish();
		IObservable<OfferSettings> observable = from _offer in ((IObservable<OfferSettings>)connectableObservable).SelectMany((Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnUnlock).Delay(dueTime)
			where _offer.LockWithTimer.IsOpen.Value
			select _offer;
		((IObservable<OfferSettings>)connectableObservable).SelectMany((Func<OfferSettings, IObservable<OfferSettings>>)EmitUnlockedOfferByTimer).Merge(observable).Subscribe(AddOfferToCollectionForPush)
			.AddTo(disposables);
		(from _offer in ((IObservable<OfferSettings>)connectableObservable).SelectMany((Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnUnlock).SelectMany((OfferSettings _offer) => EmitOfferOnLock(_offer).FirstOrDefault())
			where sectionController.CurrentSection != null && sectionController.CurrentSection.Source == _offer
			select _offer).Subscribe(delegate
		{
			windowOpener.Close();
		}).AddTo(disposables);
		connectableObservable.Connect().AddTo(disposables);
	}

	private void TrackOffersToRemoveFromPushQueue()
	{
		(from _addEvent in pushCollection.ObserveAdd()
			select _addEvent.Value).SelectMany((OfferSettings _offer) => EmitOfferOnLock(_offer).Where(pushCollection.Contains)).Subscribe(delegate(OfferSettings _offer)
		{
			pushCollection.Remove(_offer);
		}, delegate(Exception ex)
		{
			ex.LogException();
		}).AddTo(disposables);
	}

	private IObservable<OfferSettings> EmitOfferOnLock(OfferSettings _offer)
	{
		return from _ in _offer.LockWithTimer.IsOpen.Where((bool _isOpen) => !_isOpen).First()
			select _offer;
	}

	private IObservable<OfferSettings> EmitOfferOnUnlock(OfferSettings _offer)
	{
		return from x in _offer.LockWithTimer.IsOpen
			where x
			select x into _
			select _offer;
	}

	protected virtual void OnDisable()
	{
		windowsManager.OnCloseWindow -= RestartPushTimerForJustShownOffer;
		disposables.Clear();
	}

	private void OnDestroy()
	{
		disposables.Dispose();
		currentShownOffer.OnCompleted();
		currentShownOffer.Dispose();
	}
}

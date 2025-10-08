using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		((Collection<OfferSettings>)(object)pushCollection).Clear();
		if (base.isActiveAndEnabled)
		{
			CompositeDisposable obj = disposables;
			if (obj != null)
			{
				obj.Clear();
			}
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
				((Collection<OfferSettings>)(object)pushCollection).Remove(source);
				RestartTrackOnClose(source);
			}
		}
	}

	private void AddOfferToCollectionForPush(OfferSettings offer)
	{
		if (!((Collection<OfferSettings>)(object)pushCollection).Contains(offer))
		{
			IObservable<LootboxLinkedContent> observable = Observable.ToObservable<LootboxLinkedContent>(offer.Bundles.Select((BundleLot _bundle) => _bundle.Content).OfType<LootboxLinkedContent>());
			IObservable<Unit> observable2 = Observable.AsUnitObservable<ICharacter>(Observable.DefaultIfEmpty<ICharacter>(Observable.Concat<ICharacter>(Observable.Select<int, IObservable<ICharacter>>(Observable.SelectMany<LootboxLinkedContent, int>(observable, (Func<LootboxLinkedContent, IEnumerable<int>>)((LootboxLinkedContent content) => content.Lootbox.CharacterIdPossibleDrops)), (Func<int, IObservable<ICharacter>>)((int _girlIds) => _characterProvider.Get(_girlIds))))));
			IObservable<Unit> observable3 = Observable.AsUnitObservable<SkinData>(Observable.DefaultIfEmpty<SkinData>(Observable.Concat<SkinData>(Observable.Select<int, IObservable<SkinData>>(Observable.SelectMany<LootboxLinkedContent, int>(observable, (Func<LootboxLinkedContent, IEnumerable<int>>)((LootboxLinkedContent content) => content.Lootbox.SkinIdPossibleDrops)), (Func<int, IObservable<SkinData>>)((int ids) => _skinDataLoadingController.InsertDataOnLoad(ids))))));
			ObservableExtensions.Subscribe<OfferSettings>(Observable.Select<Unit, OfferSettings>(Observable.WhenAll(new IObservable<Unit>[2] { observable2, observable3 }), (Func<Unit, OfferSettings>)((Unit _) => offer)), (Action<OfferSettings>)delegate
			{
				((Collection<OfferSettings>)(object)pushCollection).Add(offer);
			});
		}
	}

	private void ShowNext()
	{
		OfferSettings offerSettings = ((Collection<OfferSettings>)(object)pushCollection)[0];
		((Collection<OfferSettings>)(object)pushCollection).RemoveAt(0);
		windowOpener.Click();
		sectionController.LoadSection(offerSettings);
		offerSettings.InvokeOnPushedEvent();
	}

	private void RestartTrackOnClose(OfferSettings offer)
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<OfferSettings>(EmitUnlockedOfferByTimer(offer), (Action<OfferSettings>)AddOfferToCollectionForPush), (ICollection<IDisposable>)disposables);
	}

	private IObservable<OfferSettings> EmitUnlockedOfferByTimer(OfferSettings offerSettings)
	{
		return Observable.Select<long, OfferSettings>(Observable.Where<long>(Observable.TakeUntil<long, OfferSettings>(Observable.TakeUntil<long, bool>(Observable.ContinueWith<bool, long>(Observable.FirstOrDefault<bool>((IObservable<bool>)offerSettings.LockWithTimer.IsOpen, (Func<bool, bool>)((bool x) => x)), Observable.Timer(offerSettings.PushTime, Scheduler.MainThreadIgnoreTimeScale)), ObservableOfferLockout(offerSettings)), Observable.FirstOrDefault<OfferSettings>((IObservable<OfferSettings>)currentShownOffer, (Func<OfferSettings, bool>)((OfferSettings _offer) => _offer != null && _offer.ID == offerSettings.ID))), (Func<long, bool>)((long _) => offerSettings.Bundles.All((BundleLot _bundle) => _bundle.IsAvailable()))), (Func<long, OfferSettings>)((long _) => offerSettings));
	}

	private IObservable<bool> ObservableOfferLockout(OfferSettings offerSettings)
	{
		return Observable.First<bool>(Observable.Skip<bool>((IObservable<bool>)offerSettings.LockWithTimer.IsOpen, 1), (Func<bool, bool>)((bool x) => !x));
	}

	private void PushOffers()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(Observable.CombineLatest<CollectionAddEvent<OfferSettings>, bool, bool>(pushCollection.ObserveAdd(), (IObservable<bool>)screenIndicator.IsVisible, (Func<CollectionAddEvent<OfferSettings>, bool, bool>)((CollectionAddEvent<OfferSettings> _addEvent, bool _isScreenVisible) => _isScreenVisible && ((IEnumerable<OfferSettings>)pushCollection).Any())), (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			ShowNext();
		}), (ICollection<IDisposable>)disposables);
	}

	private void TrackOffersToBePushed()
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(3.0);
		IConnectableObservable<OfferSettings> val = Observable.Publish<OfferSettings>(Observable.Merge<OfferSettings>(Observable.ToObservable<OfferSettings>(base.Source.Collection), new IObservable<OfferSettings>[1] { base.Source.OnNew }));
		IObservable<OfferSettings> observable = Observable.Where<OfferSettings>(Observable.Delay<OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>((IObservable<OfferSettings>)val, (Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnUnlock), timeSpan), (Func<OfferSettings, bool>)((OfferSettings _offer) => _offer.LockWithTimer.IsOpen.Value));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<OfferSettings>(Observable.Merge<OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>((IObservable<OfferSettings>)val, (Func<OfferSettings, IObservable<OfferSettings>>)EmitUnlockedOfferByTimer), new IObservable<OfferSettings>[1] { observable }), (Action<OfferSettings>)AddOfferToCollectionForPush), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<OfferSettings>(Observable.Where<OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>((IObservable<OfferSettings>)val, (Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnUnlock), (Func<OfferSettings, IObservable<OfferSettings>>)((OfferSettings _offer) => Observable.FirstOrDefault<OfferSettings>(EmitOfferOnLock(_offer)))), (Func<OfferSettings, bool>)((OfferSettings _offer) => sectionController.CurrentSection != null && sectionController.CurrentSection.Source == _offer)), (Action<OfferSettings>)delegate
		{
			windowOpener.Close();
		}), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)disposables);
	}

	private void TrackOffersToRemoveFromPushQueue()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>(Observable.Select<CollectionAddEvent<OfferSettings>, OfferSettings>(pushCollection.ObserveAdd(), (Func<CollectionAddEvent<OfferSettings>, OfferSettings>)((CollectionAddEvent<OfferSettings> _addEvent) => _addEvent.Value)), (Func<OfferSettings, IObservable<OfferSettings>>)((OfferSettings _offer) => Observable.Where<OfferSettings>(EmitOfferOnLock(_offer), (Func<OfferSettings, bool>)((Collection<OfferSettings>)(object)pushCollection).Contains))), (Action<OfferSettings>)delegate(OfferSettings _offer)
		{
			((Collection<OfferSettings>)(object)pushCollection).Remove(_offer);
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		}), (ICollection<IDisposable>)disposables);
	}

	private IObservable<OfferSettings> EmitOfferOnLock(OfferSettings _offer)
	{
		return Observable.Select<bool, OfferSettings>(Observable.First<bool>(Observable.Where<bool>((IObservable<bool>)_offer.LockWithTimer.IsOpen, (Func<bool, bool>)((bool _isOpen) => !_isOpen))), (Func<bool, OfferSettings>)((bool _) => _offer));
	}

	private IObservable<OfferSettings> EmitOfferOnUnlock(OfferSettings _offer)
	{
		return Observable.Select<bool, OfferSettings>(Observable.Where<bool>((IObservable<bool>)_offer.LockWithTimer.IsOpen, (Func<bool, bool>)((bool x) => x)), (Func<bool, OfferSettings>)((bool _) => _offer));
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

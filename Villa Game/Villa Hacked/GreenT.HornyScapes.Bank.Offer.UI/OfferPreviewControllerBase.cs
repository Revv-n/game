using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins;
using StripClub.Model;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using StripClub.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Bank.Offer.UI;

public abstract class OfferPreviewControllerBase : MonoView<IEnumerable<OfferSettings>>
{
	private CompositeDisposable disposables = new CompositeDisposable();

	private Subject<OfferSettings> offerLockedAfterPurchase = new Subject<OfferSettings>();

	[Inject]
	private CharacterProvider _characterProvider;

	[Inject]
	private SkinDataLoadingController _skinDataLoadingController;

	protected List<OfferSettings> VisibleOffers { get; private set; } = new List<OfferSettings>();


	public IEnumerable<OfferSettings> GetVisibleOffers => VisibleOffers.AsEnumerable();

	public override void Set(IEnumerable<OfferSettings> source)
	{
		base.Set(source);
		Launch();
	}

	public void Launch()
	{
		if (base.Source == null)
		{
			return;
		}
		disposables.Clear();
		List<OfferSettings> list = new List<OfferSettings>(VisibleOffers);
		VisibleOffers.Clear();
		foreach (OfferSettings item in list)
		{
			Hide(item);
		}
		IConnectableObservable<OfferSettings> val = Observable.Publish<OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>(Observable.ToObservable<OfferSettings>(base.Source), (Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnOpen));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>(Observable.Where<OfferSettings>(Observable.Where<OfferSettings>((IObservable<OfferSettings>)val, (Func<OfferSettings, bool>)DisplayOfferImmidiate), (Func<OfferSettings, bool>)((OfferSettings _offer) => !VisibleOffers.Contains(_offer))), (Func<OfferSettings, IObservable<OfferSettings>>)((OfferSettings _offer) => Observable.Take<OfferSettings>(AddToVisibles(_offer), 1))), (Action<OfferSettings>)OnOfferUnlocked), (ICollection<IDisposable>)disposables);
		IObservable<OfferSettings> observable = Observable.Share<OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>((IObservable<OfferSettings>)val, (Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnLock));
		IObservable<OfferSettings> observable2 = Observable.SelectMany<OfferSettings, OfferSettings>(Observable.Where<OfferSettings>(observable, (Func<OfferSettings, bool>)((OfferSettings _offer) => VisibleOffers.Contains(_offer) && !_offer.ForcedLock)), (Func<OfferSettings, IObservable<OfferSettings>>)((OfferSettings _offer) => Observable.Select<bool, OfferSettings>(Observable.Where<bool>((IObservable<bool>)_offer.DisplayTimeLocker.IsOpen, (Func<bool, bool>)((bool x) => !x)), (Func<bool, OfferSettings>)((bool _) => _offer))));
		IObservable<(OfferSettings, OfferSettings)> observable3 = Observable.Share<(OfferSettings, OfferSettings)>(Observable.Select<OfferSettings, (OfferSettings, OfferSettings)>(Observable.DelayFrame<OfferSettings>(Observable.Do<OfferSettings>(Observable.Merge<OfferSettings>(Observable.Merge<OfferSettings>(Observable.Where<OfferSettings>(observable, (Func<OfferSettings, bool>)((OfferSettings _offer) => VisibleOffers.Contains(_offer) && _offer.ForcedLock)), new IObservable<OfferSettings>[1] { observable2 }), new IObservable<OfferSettings>[1] { (IObservable<OfferSettings>)offerLockedAfterPurchase }), (Action<OfferSettings>)delegate(OfferSettings _offer)
		{
			VisibleOffers.Remove(_offer);
		}), 1, (FrameCountType)0), (Func<OfferSettings, (OfferSettings, OfferSettings)>)((OfferSettings _offer) => (current: _offer, next: GetNextOfferOnPositionOrDefault(_offer)))));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(OfferSettings, OfferSettings)>(Observable.SelectMany<(OfferSettings, OfferSettings), (OfferSettings, OfferSettings)>(Observable.Where<(OfferSettings, OfferSettings)>(observable3, (Func<(OfferSettings, OfferSettings), bool>)(((OfferSettings current, OfferSettings next) x) => x.next != null)), (Func<(OfferSettings, OfferSettings), IObservable<(OfferSettings, OfferSettings)>>)(((OfferSettings current, OfferSettings next) _offer) => Observable.Take<(OfferSettings, OfferSettings)>(AddToVisibles(_offer), 1))), (Action<(OfferSettings, OfferSettings)>)delegate((OfferSettings current, OfferSettings next) _tupple)
		{
			Replace(_tupple.current, _tupple.next);
		}), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<(OfferSettings, OfferSettings)>(Observable.Where<(OfferSettings, OfferSettings)>(observable3, (Func<(OfferSettings, OfferSettings), bool>)(((OfferSettings current, OfferSettings next) x) => x.next == null)), (Action<(OfferSettings, OfferSettings)>)delegate((OfferSettings current, OfferSettings next) _tupple)
		{
			Hide(_tupple.current);
		}), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)disposables);
	}

	protected virtual void Hide(OfferSettings current)
	{
	}

	protected virtual void Replace(OfferSettings offer, OfferSettings next)
	{
	}

	private IObservable<(OfferSettings current, OfferSettings next)> AddToVisibles((OfferSettings current, OfferSettings next) pair)
	{
		return Observable.Select<OfferSettings, (OfferSettings, OfferSettings)>(AddToVisibles(pair.next), (Func<OfferSettings, (OfferSettings, OfferSettings)>)((OfferSettings _) => pair));
	}

	private IObservable<OfferSettings> AddToVisibles(OfferSettings offer)
	{
		IObservable<LootboxLinkedContent> observable = Observable.ToObservable<LootboxLinkedContent>(offer.Bundles.Select((BundleLot _bundle) => _bundle.Content).OfType<LootboxLinkedContent>());
		IObservable<Unit> observable2 = Observable.AsUnitObservable<ICharacter>(Observable.DefaultIfEmpty<ICharacter>(Observable.Concat<ICharacter>(Observable.Select<int, IObservable<ICharacter>>(Observable.SelectMany<LootboxLinkedContent, int>(observable, (Func<LootboxLinkedContent, IEnumerable<int>>)((LootboxLinkedContent content) => content.Lootbox.CharacterIdPossibleDrops)), (Func<int, IObservable<ICharacter>>)((int _girlIds) => _characterProvider.Get(_girlIds))))));
		IObservable<Unit> observable3 = Observable.AsUnitObservable<SkinData>(Observable.DefaultIfEmpty<SkinData>(Observable.Concat<SkinData>(Observable.Select<int, IObservable<SkinData>>(Observable.SelectMany<LootboxLinkedContent, int>(observable, (Func<LootboxLinkedContent, IEnumerable<int>>)((LootboxLinkedContent content) => content.Lootbox.SkinIdPossibleDrops)), (Func<int, IObservable<SkinData>>)((int ids) => _skinDataLoadingController.InsertDataOnLoad(ids))))));
		return Observable.Do<OfferSettings>(Observable.Select<Unit, OfferSettings>(Observable.WhenAll(new IObservable<Unit>[2] { observable2, observable3 }), (Func<Unit, OfferSettings>)((Unit _) => offer)), (Action<OfferSettings>)delegate
		{
			VisibleOffers.Add(offer);
			VisibleOffers.Sort(OfferCompareBySortingNumber);
		});
	}

	protected virtual bool DisplayOfferImmidiate(OfferSettings offer)
	{
		if (VisibleOffers.All((OfferSettings _offer) => _offer.SortingNumber != offer.SortingNumber))
		{
			return offer.Bundles.All((BundleLot _bundle) => _bundle.IsAvailable());
		}
		return false;
	}

	public abstract void Display(OfferSettings offer);

	protected void DisplayNextOffer(int currentNumber)
	{
		OfferSettings offer = SelectNextOffer(currentNumber);
		Display(offer);
	}

	protected OfferSettings GetNextOfferOnPositionOrDefault(OfferSettings offer)
	{
		return base.Source.FirstOrDefault((OfferSettings _offer) => _offer.LockWithTimer.IsOpen.Value && _offer.SortingNumber == offer.SortingNumber && _offer.Bundles.All((BundleLot _bundle) => _bundle.IsAvailable()));
	}

	protected virtual void OnOfferUnlocked(OfferSettings offer)
	{
		Display(offer);
	}

	protected static int OfferCompareBySortingNumber(OfferSettings offer, OfferSettings other)
	{
		return offer.SortingNumber - other.SortingNumber;
	}

	public void OnLotBoughtRequest(LotBoughtSignal signal)
	{
		if (signal.Lot.IsAvailable())
		{
			return;
		}
		OfferSettings[] array = VisibleOffers.Where((OfferSettings _offer) => _offer.Bundles.Any((BundleLot _bundle) => _bundle == signal.Lot && !_bundle.IsAvailable())).ToArray();
		foreach (OfferSettings offerSettings in array)
		{
			offerLockedAfterPurchase.OnNext(offerSettings);
		}
	}

	protected virtual OfferSettings SelectNextOffer(int sortingNumber, bool forward = true)
	{
		if (!VisibleOffers.Any())
		{
			return null;
		}
		if (forward)
		{
			return VisibleOffers.FirstOrDefault((OfferSettings _offer) => _offer.SortingNumber > sortingNumber) ?? VisibleOffers[0];
		}
		return VisibleOffers.LastOrDefault((OfferSettings _offer) => _offer.SortingNumber < sortingNumber) ?? VisibleOffers[VisibleOffers.Count - 1];
	}

	protected virtual void OnDestroy()
	{
		disposables.Dispose();
		offerLockedAfterPurchase.OnCompleted();
		offerLockedAfterPurchase.Dispose();
	}

	public static IObservable<OfferSettings> EmitOfferOnCondition(OfferSettings offer, Func<bool, bool> condition)
	{
		return Observable.Select<bool, OfferSettings>(Observable.Where<bool>((IObservable<bool>)offer.LockWithTimer.IsOpen, condition), (Func<bool, OfferSettings>)((bool _) => offer));
	}

	public static IObservable<OfferSettings> EmitOfferOnOpen(OfferSettings offer)
	{
		return EmitOfferOnCondition(offer, (bool x) => x);
	}

	public static IObservable<OfferSettings> EmitOfferOnLock(OfferSettings offer)
	{
		return EmitOfferOnCondition(offer, (bool x) => !x);
	}
}

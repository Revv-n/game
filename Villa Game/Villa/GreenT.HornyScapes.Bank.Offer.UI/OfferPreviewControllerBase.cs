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
		IConnectableObservable<OfferSettings> connectableObservable = base.Source.ToObservable().SelectMany((Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnOpen).Publish();
		(from _offer in connectableObservable.Where(DisplayOfferImmidiate)
			where !VisibleOffers.Contains(_offer)
			select _offer).SelectMany((OfferSettings _offer) => AddToVisibles(_offer).Take(1)).Subscribe(OnOfferUnlocked).AddTo(disposables);
		IObservable<OfferSettings> source = ((IObservable<OfferSettings>)connectableObservable).SelectMany((Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnLock).Share();
		IObservable<OfferSettings> observable = source.Where((OfferSettings _offer) => VisibleOffers.Contains(_offer) && !_offer.ForcedLock).SelectMany((OfferSettings _offer) => from x in _offer.DisplayTimeLocker.IsOpen
			where !x
			select x into _
			select _offer);
		IObservable<(OfferSettings current, OfferSettings next)> source2 = (from _offer in source.Where((OfferSettings _offer) => VisibleOffers.Contains(_offer) && _offer.ForcedLock).Merge(observable).Merge(offerLockedAfterPurchase)
				.Do(delegate(OfferSettings _offer)
				{
					VisibleOffers.Remove(_offer);
				})
				.DelayFrame(1)
			select (current: _offer, next: GetNextOfferOnPositionOrDefault(_offer))).Share();
		source2.Where(((OfferSettings current, OfferSettings next) x) => x.next != null).SelectMany(((OfferSettings current, OfferSettings next) _offer) => AddToVisibles(_offer).Take(1)).Subscribe(delegate((OfferSettings current, OfferSettings next) _tupple)
		{
			Replace(_tupple.current, _tupple.next);
		})
			.AddTo(disposables);
		source2.Where(((OfferSettings current, OfferSettings next) x) => x.next == null).Subscribe(delegate((OfferSettings current, OfferSettings next) _tupple)
		{
			Hide(_tupple.current);
		}).AddTo(disposables);
		connectableObservable.Connect().AddTo(disposables);
	}

	protected virtual void Hide(OfferSettings current)
	{
	}

	protected virtual void Replace(OfferSettings offer, OfferSettings next)
	{
	}

	private IObservable<(OfferSettings current, OfferSettings next)> AddToVisibles((OfferSettings current, OfferSettings next) pair)
	{
		return from _ in AddToVisibles(pair.next)
			select pair;
	}

	private IObservable<OfferSettings> AddToVisibles(OfferSettings offer)
	{
		IObservable<LootboxLinkedContent> source = offer.Bundles.Select((BundleLot _bundle) => _bundle.Content).OfType<LootboxLinkedContent>().ToObservable();
		IObservable<Unit> observable = (from _girlIds in source.SelectMany((LootboxLinkedContent content) => content.Lootbox.CharacterIdPossibleDrops)
			select _characterProvider.Get(_girlIds)).Concat().DefaultIfEmpty().AsUnitObservable();
		IObservable<Unit> observable2 = (from ids in source.SelectMany((LootboxLinkedContent content) => content.Lootbox.SkinIdPossibleDrops)
			select _skinDataLoadingController.InsertDataOnLoad(ids)).Concat().DefaultIfEmpty().AsUnitObservable();
		return (from _ in Observable.WhenAll(observable, observable2)
			select offer).Do(delegate
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
		foreach (OfferSettings value in array)
		{
			offerLockedAfterPurchase.OnNext(value);
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
		return from _ in offer.LockWithTimer.IsOpen.Where(condition)
			select offer;
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

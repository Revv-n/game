using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Offer;

public class OfferUnlockController : MonoView<IEnumerable<OfferSettings>>
{
	private CompositeDisposable disposables = new CompositeDisposable();

	private GameStarter gameStarter;

	[Inject]
	public void Init(GameStarter gameStarter)
	{
		this.gameStarter = gameStarter;
	}

	public override void Set(IEnumerable<OfferSettings> source)
	{
		base.Set(source);
		Launch();
	}

	public void Launch()
	{
		if (base.Source == null)
		{
			Debug.LogError("Source weren't set yet");
			return;
		}
		disposables.Clear();
		IEnumerable<OfferSettings> offers = base.Source;
		IConnectableObservable<OfferSettings> connectableObservable = gameStarter.IsGameActive.FirstOrDefault((bool x) => x).ContinueWith((bool _) => offers.Select(EmitOfferOnLockersOpen).Merge()).Publish();
		IObservable<OfferSettings> first = connectableObservable.Where((OfferSettings x) => !AnyOfferShownOnCertainPosition(x));
		IObservable<OfferSettings> observable = connectableObservable.Where(AnyOfferShownOnCertainPosition).SelectMany((Func<OfferSettings, IObservable<OfferSettings>>)EmitOnOtherOffersLocked);
		IConnectableObservable<OfferSettings> connectableObservable2 = first.Merge(observable).Publish();
		connectableObservable2.Subscribe(LaunchOfferTimer).AddTo(disposables);
		IObservable<OfferSettings> source = ((IObservable<OfferSettings>)connectableObservable2).SelectMany((Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnLockersClosed).Share();
		IObservable<OfferSettings> observable2 = source.Where((OfferSettings _offer) => !_offer.ForcedLock).SelectMany((OfferSettings _offer) => from _ in _offer.DisplayTimeLocker.IsOpen.FirstOrDefault((bool x) => !x)
			select _offer);
		source.Where((OfferSettings _offer) => _offer.ForcedLock).Merge(observable2).Subscribe(StopOfferTimer)
			.AddTo(disposables);
		connectableObservable2.Connect().AddTo(disposables);
		connectableObservable.Connect().AddTo(disposables);
	}

	private IObservable<OfferSettings> EmitOnOtherOffersLocked(OfferSettings offer)
	{
		return from _ in (from _offer in base.Source.Where((OfferSettings _other) => _other.SortingNumber == offer.SortingNumber).TakeWhile((OfferSettings _other) => _other.ID != offer.ID)
				select _offer.LockWithTimer.IsOpen).CombineLatest().FirstOrDefault((IList<bool> _stateList) => _stateList.All((bool _isOpen) => !_isOpen))
			select offer;
	}

	private bool AnyOfferShownOnCertainPosition(OfferSettings offer)
	{
		return base.Source.Any((OfferSettings _other) => _other.ID != offer.ID && _other.SortingNumber == offer.SortingNumber && _other.LockWithTimer.IsOpen.Value);
	}

	private void LaunchOfferTimer(OfferSettings offer)
	{
		offer.LaunchTimers();
	}

	private void StopOfferTimer(OfferSettings offer)
	{
		offer.StopTimers();
	}

	public static IObservable<OfferSettings> EmitOfferOnCondition(OfferSettings offer, Func<bool, bool> condition)
	{
		return from _ in offer.Lock.IsOpen.Where(condition)
			select offer;
	}

	public static IObservable<OfferSettings> EmitOfferOnLockersOpen(OfferSettings offer)
	{
		return EmitOfferOnCondition(offer, (bool x) => x);
	}

	public static IObservable<OfferSettings> EmitOfferOnLockersClosed(OfferSettings offer)
	{
		return EmitOfferOnCondition(offer, (bool x) => !x);
	}

	protected virtual void OnDestroy()
	{
		disposables.Dispose();
	}
}

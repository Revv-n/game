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
		IConnectableObservable<OfferSettings> val = Observable.Publish<OfferSettings>(Observable.ContinueWith<bool, OfferSettings>(Observable.FirstOrDefault<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Func<bool, IObservable<OfferSettings>>)((bool _) => Observable.Merge<OfferSettings>(offers.Select(EmitOfferOnLockersOpen)))));
		IObservable<OfferSettings> observable = Observable.Where<OfferSettings>((IObservable<OfferSettings>)val, (Func<OfferSettings, bool>)((OfferSettings x) => !AnyOfferShownOnCertainPosition(x)));
		IObservable<OfferSettings> observable2 = Observable.SelectMany<OfferSettings, OfferSettings>(Observable.Where<OfferSettings>((IObservable<OfferSettings>)val, (Func<OfferSettings, bool>)AnyOfferShownOnCertainPosition), (Func<OfferSettings, IObservable<OfferSettings>>)EmitOnOtherOffersLocked);
		IConnectableObservable<OfferSettings> val2 = Observable.Publish<OfferSettings>(Observable.Merge<OfferSettings>(observable, new IObservable<OfferSettings>[1] { observable2 }));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<OfferSettings>((IObservable<OfferSettings>)val2, (Action<OfferSettings>)LaunchOfferTimer), (ICollection<IDisposable>)disposables);
		IObservable<OfferSettings> observable3 = Observable.Share<OfferSettings>(Observable.SelectMany<OfferSettings, OfferSettings>((IObservable<OfferSettings>)val2, (Func<OfferSettings, IObservable<OfferSettings>>)EmitOfferOnLockersClosed));
		IObservable<OfferSettings> observable4 = Observable.SelectMany<OfferSettings, OfferSettings>(Observable.Where<OfferSettings>(observable3, (Func<OfferSettings, bool>)((OfferSettings _offer) => !_offer.ForcedLock)), (Func<OfferSettings, IObservable<OfferSettings>>)((OfferSettings _offer) => Observable.Select<bool, OfferSettings>(Observable.FirstOrDefault<bool>((IObservable<bool>)_offer.DisplayTimeLocker.IsOpen, (Func<bool, bool>)((bool x) => !x)), (Func<bool, OfferSettings>)((bool _) => _offer))));
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<OfferSettings>(Observable.Merge<OfferSettings>(Observable.Where<OfferSettings>(observable3, (Func<OfferSettings, bool>)((OfferSettings _offer) => _offer.ForcedLock)), new IObservable<OfferSettings>[1] { observable4 }), (Action<OfferSettings>)StopOfferTimer), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(val2.Connect(), (ICollection<IDisposable>)disposables);
		DisposableExtensions.AddTo<IDisposable>(val.Connect(), (ICollection<IDisposable>)disposables);
	}

	private IObservable<OfferSettings> EmitOnOtherOffersLocked(OfferSettings offer)
	{
		return Observable.Select<IList<bool>, OfferSettings>(Observable.FirstOrDefault<IList<bool>>(Observable.CombineLatest<bool>((IEnumerable<IObservable<bool>>)(from _offer in base.Source.Where((OfferSettings _other) => _other.SortingNumber == offer.SortingNumber).TakeWhile((OfferSettings _other) => _other.ID != offer.ID)
			select _offer.LockWithTimer.IsOpen)), (Func<IList<bool>, bool>)((IList<bool> _stateList) => _stateList.All((bool _isOpen) => !_isOpen))), (Func<IList<bool>, OfferSettings>)((IList<bool> _) => offer));
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
		return Observable.Select<bool, OfferSettings>(Observable.Where<bool>((IObservable<bool>)offer.Lock.IsOpen, condition), (Func<bool, OfferSettings>)((bool _) => offer));
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

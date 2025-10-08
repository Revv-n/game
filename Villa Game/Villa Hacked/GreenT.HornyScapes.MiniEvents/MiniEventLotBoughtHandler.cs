using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Shop;
using StripClub.Model.Shop.Data;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventLotBoughtHandler : IDisposable
{
	private readonly SignalBus _signalBus;

	private readonly CompositeDisposable _trackStream;

	private readonly LotManager _lotManager;

	private IEnumerable<Lot> _currentLots;

	private IDisposable _onUnlockDisposable;

	private Action _updateCurrentTab;

	public MiniEventLotBoughtHandler(LotManager lotManager, SignalBus signalBus)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		_lotManager = lotManager;
		_signalBus = signalBus;
		_trackStream = new CompositeDisposable();
	}

	public void Init(Action updateCurrentTab)
	{
		_updateCurrentTab = updateCurrentTab;
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Lot>(Observable.Where<Lot>(Observable.Select<LotBoughtSignal, Lot>(_signalBus.GetStream<LotBoughtSignal>(), (Func<LotBoughtSignal, Lot>)((LotBoughtSignal _signal) => _signal.Lot)), (Func<Lot, bool>)((Lot _lot) => _currentLots == null || !_currentLots.Any() || _currentLots.Any((Lot lot) => lot.TabID == _lot.TabID))), (Action<Lot>)delegate
		{
			updateCurrentTab();
		}), (ICollection<IDisposable>)_trackStream);
	}

	public void Dispose()
	{
		_trackStream.Clear();
		_trackStream.Dispose();
		_onUnlockDisposable?.Dispose();
	}

	public void SetupLots(IEnumerable<Lot> lots)
	{
		_currentLots = lots;
		_onUnlockDisposable?.Dispose();
		_onUnlockDisposable = ObservableExtensions.Subscribe<Lot>(Observable.Where<Lot>(Observable.SelectMany<Lot, Lot>(Observable.ToObservable<Lot>(_lotManager.Collection), (Func<Lot, IObservable<Lot>>)EmitOnUnlock), (Func<Lot, bool>)((Lot currentLot) => _currentLots.Any((Lot lot) => lot.TabID == currentLot.TabID))), (Action<Lot>)delegate
		{
			_updateCurrentTab();
		});
	}

	private IObservable<Lot> EmitOnUnlock(Lot lot)
	{
		return Observable.Select<bool, Lot>(Observable.Skip<bool>((IObservable<bool>)lot.Locker.IsOpen, 1), (Func<bool, Lot>)((bool _) => lot));
	}
}

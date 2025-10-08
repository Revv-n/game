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
		_lotManager = lotManager;
		_signalBus = signalBus;
		_trackStream = new CompositeDisposable();
	}

	public void Init(Action updateCurrentTab)
	{
		_updateCurrentTab = updateCurrentTab;
		(from _signal in _signalBus.GetStream<LotBoughtSignal>()
			select _signal.Lot into _lot
			where _currentLots == null || !_currentLots.Any() || _currentLots.Any((Lot lot) => lot.TabID == _lot.TabID)
			select _lot).Subscribe(delegate
		{
			updateCurrentTab();
		}).AddTo(_trackStream);
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
		_onUnlockDisposable = (from currentLot in _lotManager.Collection.ToObservable().SelectMany((Func<Lot, IObservable<Lot>>)EmitOnUnlock)
			where _currentLots.Any((Lot lot) => lot.TabID == currentLot.TabID)
			select currentLot).Subscribe(delegate
		{
			_updateCurrentTab();
		});
	}

	private IObservable<Lot> EmitOnUnlock(Lot lot)
	{
		return from _ in lot.Locker.IsOpen.Skip(1)
			select lot;
	}
}

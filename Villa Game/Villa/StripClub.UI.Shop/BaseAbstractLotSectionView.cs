using System;
using System.Collections.Generic;
using System.Linq;
using GreenT;
using GreenT.HornyScapes.Bank;
using Zenject;

namespace StripClub.UI.Shop;

public abstract class BaseAbstractLotSectionView<T, L> : MonoView<T> where T : IBankSection
{
	protected IManager<L> _lotManager;

	protected IEnumerable<L> _targetLots;

	protected IEnumerable<L> _visibleLots;

	protected IDisposable _updateStream;

	[Inject]
	public void Init(IManager<L> lotManager)
	{
		_lotManager = lotManager;
	}

	public override void Set(T source)
	{
		base.Set(source);
		_targetLots = TargetLots(_lotManager.Collection).ToList();
		_visibleLots = VisibleLots(_targetLots);
		TrackLotLockers(_targetLots);
		DisplayAvailableLots(_visibleLots);
	}

	public void ForceSet(T source)
	{
		base.Set(source);
		_targetLots = TargetLots(_lotManager.Collection).ToList();
		_visibleLots = _targetLots;
		TrackLotLockers(_targetLots);
		DisplayCloneLots(_visibleLots);
	}

	protected abstract void DisplayCloneLots(IEnumerable<L> lots);

	protected abstract void DisplayAvailableLots(IEnumerable<L> lots);

	protected abstract void ExceptionReport(IEnumerable<L> lots);

	protected abstract IEnumerable<L> TargetLots(IEnumerable<L> collection);

	protected abstract IEnumerable<L> VisibleLots(IEnumerable<L> targetLots);

	protected abstract void TrackLotLockers(IEnumerable<L> allSectionLots);

	protected virtual void OnDisable()
	{
		_updateStream?.Dispose();
	}
}

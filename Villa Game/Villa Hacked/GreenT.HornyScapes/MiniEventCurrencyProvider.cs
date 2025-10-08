using System;
using System.Collections.Generic;
using GreenT.Types;
using StripClub.Model;

namespace GreenT.HornyScapes;

public class MiniEventCurrencyProvider : IDisposable
{
	private readonly Currencies _mainBalance;

	private readonly TrackableCurrencyActionContainerTracker _trackableCurrencyActionContainerTracker;

	private Dictionary<CompositeIdentificator, TrackableCurrencyActionContainer> _containers;

	public MiniEventCurrencyProvider(Currencies mainBalance, TrackableCurrencyActionContainerTracker trackableCurrencyActionContainerTracker)
	{
		_mainBalance = mainBalance;
		_trackableCurrencyActionContainerTracker = trackableCurrencyActionContainerTracker;
		_containers = new Dictionary<CompositeIdentificator, TrackableCurrencyActionContainer>();
	}

	public void Dispose()
	{
		foreach (TrackableCurrencyActionContainer value in _containers.Values)
		{
			value.Dispose();
		}
	}

	public bool TryGetContainer(CompositeIdentificator compositeIdentificator, out ICurrenciesActionContainer container)
	{
		if (compositeIdentificator.Identificators == null)
		{
			container = null;
			return false;
		}
		if (!_containers.TryGetValue(compositeIdentificator, out var value))
		{
			value = new TrackableCurrencyActionContainer(CurrencyType.MiniEvent, _mainBalance, compositeIdentificator);
			_trackableCurrencyActionContainerTracker.OnNewContainer(value);
		}
		container = value;
		_containers.TryAdd(compositeIdentificator, value);
		return true;
	}
}

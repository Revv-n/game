using System;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

[Objective]
public class AnyCurrencyAddObjective : CurrencyAddObjective
{
	private readonly TrackableCurrencyActionContainerTracker _trackableCurrencyActionContainerTracker;

	public AnyCurrencyAddObjective(TrackableCurrencyActionContainerTracker trackableCurrencyActionContainerTracker, Func<Sprite> iconProvider, CurrencyType currencyType, SavableObjectiveData data, ICurrencyProcessor currencyProcessor)
		: base(iconProvider, currencyType, data, currencyProcessor, null)
	{
		_trackableCurrencyActionContainerTracker = trackableCurrencyActionContainerTracker;
	}

	public override void Track()
	{
		base.Track();
		_currencyStream?.Dispose();
		_currencyStream = ObservableExtensions.Subscribe<int>(_trackableCurrencyActionContainerTracker.GetAnyAddStream(_currencyType), (Action<int>)AddProgress);
	}
}

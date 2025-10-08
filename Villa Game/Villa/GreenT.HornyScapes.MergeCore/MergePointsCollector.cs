using GreenT.HornyScapes.Analytics;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class MergePointsCollector
{
	private readonly MergePointsIconService _iconService;

	private readonly CollectCurrencyServiceForFly _currencyServiceForFly;

	public MergePointsCollector(MergePointsIconService iconService, CollectCurrencyServiceForFly currencyServiceForFly)
	{
		_iconService = iconService;
		_currencyServiceForFly = currencyServiceForFly;
	}

	public void CollectMergePoints(GameItem item)
	{
		if (MergePointsManager.TryGetMergePointsModule(item, out var mergePoints))
		{
			Sprite icon = _iconService.GetIcon(mergePoints.CurrencyType, mergePoints.Identificator);
			_currencyServiceForFly.Collect(item, mergePoints.CurrencyType, mergePoints.Count, CurrencyAmplitudeAnalytic.SourceType.MergePoints, mergePoints.Identificator, icon);
		}
	}
}

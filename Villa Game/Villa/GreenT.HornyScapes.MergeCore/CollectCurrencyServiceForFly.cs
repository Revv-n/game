using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Content;
using GreenT.Types;
using Merge;
using Merge.MotionDesign;
using StripClub.Model;
using StripClub.Model.Shop;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class CollectCurrencyServiceForFly
{
	private readonly CurrencyFlyTweenBuilder _collectFlyTargetsContainer;

	private readonly SoundController _soundController;

	private readonly IPurchaseProcessor _purchaseProcessing;

	private readonly CurrencyContentFactory _currencyFactory;

	private readonly LinkedContentAnalyticDataFactory _analyticDataFactory;

	public CollectCurrencyServiceForFly(CurrencyFlyTweenBuilder collectFlyTargetsContainer, SoundController soundController, IPurchaseProcessor purchaseProcessing, CurrencyContentFactory currencyFactory, LinkedContentAnalyticDataFactory analyticDataFactory)
	{
		_collectFlyTargetsContainer = collectFlyTargetsContainer;
		_soundController = soundController;
		_purchaseProcessing = purchaseProcessing;
		_currencyFactory = currencyFactory;
		_analyticDataFactory = analyticDataFactory;
	}

	public void Collect(GameItem gameItem, CurrencyType currencyType, int count, CurrencyAmplitudeAnalytic.SourceType analyticType, CompositeIdentificator identificator = default(CompositeIdentificator), Sprite sprite = null)
	{
		int count2 = count * 2;
		_collectFlyTargetsContainer.FlyManyCurrency(gameItem, currencyType, count2, identificator, sprite);
		AtCollectCurrency(count, currencyType, identificator, analyticType);
	}

	private void AtCollectCurrency(int count, CurrencyType currencyType, CompositeIdentificator identificator, CurrencyAmplitudeAnalytic.SourceType analyticType)
	{
		LinkedContentAnalyticData analyticData = _analyticDataFactory.Create(analyticType);
		CurrencyLinkedContent content = _currencyFactory.Create(count, currencyType, analyticData, identificator);
		_purchaseProcessing.AddAllContentToPlayer(content);
		_soundController.PlayCurrencySound(currencyType);
	}
}

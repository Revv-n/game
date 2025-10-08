using GreenT.HornyScapes;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.Model;

public class CurrencySpecialLinkedContent : CurrencyLinkedContent
{
	public Sprite Sprite { get; }

	public string LocalizationKey { get; }

	public CurrencySpecialLinkedContent(int quantity, CurrencyType currency, Sprite sprite, string localizationKey, LinkedContentAnalyticData analyticData, GreenT.HornyScapes.GameSettings settings, IPlayerExpController playerExpController, GreenT.HornyScapes.IPlayerBasics playerBasics, CurrencyAnalyticProcessingService currencyAmplitudeAnalytic, ICurrencyProcessor currencyProcessor, CompositeIdentificator compositeIdentificator, LinkedContent next = null)
		: base(quantity, currency, analyticData, settings, playerExpController, playerBasics, currencyAmplitudeAnalytic, currencyProcessor, compositeIdentificator, next)
	{
		Sprite = sprite;
		LocalizationKey = localizationKey;
	}

	public override Sprite GetIcon()
	{
		return Sprite ?? base.GetIcon();
	}

	public override string GetName()
	{
		return LocalizationKey ?? base.GetName();
	}

	public override LinkedContent Clone()
	{
		return new CurrencySpecialLinkedContent(base.Quantity, base.Currency, Sprite, LocalizationKey, AnalyticData, settings, playerExpController, playerBasics, currencyAmplitudeAnalytic, currencyProcessor, base.CompositeIdentificator, base.CloneOfNext);
	}
}

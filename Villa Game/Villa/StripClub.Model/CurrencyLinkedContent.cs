using System;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Analytics;
using GreenT.Types;
using StripClub.Model.Cards;
using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.Model;

public class CurrencyLinkedContent : LinkedContent
{
	[Serializable]
	public class CurrencyMap : Map
	{
		[SerializeField]
		private CurrencyType type;

		[SerializeField]
		private int quantity;

		[SerializeField]
		private CompositeIdentificator identificator;

		public CurrencyType Type => type;

		public int Quantity => quantity;

		public CompositeIdentificator CompositeIdentificator => identificator;

		public CurrencyMap(CurrencyLinkedContent source)
			: base(source)
		{
			type = source.Currency;
			quantity = source.Quantity;
			identificator = source.CompositeIdentificator;
		}
	}

	protected readonly IPlayerExpController playerExpController;

	protected readonly GreenT.HornyScapes.IPlayerBasics playerBasics;

	protected readonly GreenT.HornyScapes.GameSettings settings;

	protected readonly CurrencyAnalyticProcessingService currencyAmplitudeAnalytic;

	protected readonly ICurrencyProcessor currencyProcessor;

	public CurrencyType Currency { get; protected set; }

	public CompositeIdentificator CompositeIdentificator { get; protected set; }

	public int Quantity { get; protected set; }

	public override Type Type => typeof(CurrencyLinkedContent);

	public CurrencyLinkedContent(int quantity, CurrencyType currency, LinkedContentAnalyticData analyticData, GreenT.HornyScapes.GameSettings settings, IPlayerExpController playerExpController, GreenT.HornyScapes.IPlayerBasics playerBasics, CurrencyAnalyticProcessingService currencyAmplitudeAnalytic, ICurrencyProcessor currencyProcessor, CompositeIdentificator compositeIdentificator, LinkedContent next = null)
		: base(analyticData, next)
	{
		Currency = currency;
		CompositeIdentificator = compositeIdentificator;
		this.playerExpController = playerExpController;
		this.playerBasics = playerBasics;
		this.currencyAmplitudeAnalytic = currencyAmplitudeAnalytic;
		this.currencyProcessor = currencyProcessor;
		Quantity = quantity;
		this.settings = settings;
	}

	public override Sprite GetIcon()
	{
		return settings.CurrencySettings[Currency, CompositeIdentificator].Sprite;
	}

	public override bool TryGetAlternativeIcon(out Sprite sprite)
	{
		sprite = null;
		if (settings.CurrencySettings[Currency, CompositeIdentificator].AlternativeSprite == null)
		{
			return false;
		}
		sprite = settings.CurrencySettings[Currency, CompositeIdentificator].AlternativeSprite;
		return sprite != null;
	}

	public override Sprite GetProgressBarIcon()
	{
		return settings.CurrencySettings[Currency, CompositeIdentificator].AlternativeSprite;
	}

	public override Rarity GetRarity()
	{
		return Rarity.Common;
	}

	public override string GetName()
	{
		if (Currency != CurrencyType.MiniEvent)
		{
			return settings.CurrencySettings[Currency, CompositeIdentificator].Key;
		}
		return string.Format(settings.CurrencySettings[Currency, CompositeIdentificator].Key, CompositeIdentificator[0]);
	}

	public override string GetDescription()
	{
		return Quantity.ToString();
	}

	public override string ToString()
	{
		return Quantity + " " + Currency;
	}

	public override Map GetMap()
	{
		return new CurrencyMap(this);
	}

	public override void AddCurrentToPlayer()
	{
		currencyProcessor.TryAdd(Currency, Quantity, AnalyticData.SourceType, CompositeIdentificator);
		currencyAmplitudeAnalytic.SendCurrencyEvent(this);
		base.AddCurrentToPlayer();
	}

	public override LinkedContent Clone()
	{
		return new CurrencyLinkedContent(Quantity, Currency, AnalyticData, settings, playerExpController, playerBasics, currencyAmplitudeAnalytic, currencyProcessor, CompositeIdentificator, base.CloneOfNext);
	}
}

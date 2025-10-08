using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Presents.Analytics;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Presents.Models;

public class PresentLinkedContent : LinkedContent
{
	[Serializable]
	public class PresentMap : Map
	{
		public string ID;

		public int Quantity;

		public PresentMap(PresentLinkedContent source)
			: base(source)
		{
			ID = source.Present.Id;
			Quantity = source.Quantity;
		}
	}

	private const string key = "present.";

	private readonly PresentsAnalytic _presentsAnalytic;

	public int Quantity { get; }

	public Present Present { get; }

	public CompositeIdentificator CompositeIdentificator { get; private set; }

	public override Type Type => typeof(PresentLinkedContent);

	public PresentLinkedContent(Present present, int quantity, LinkedContentAnalyticData analyticData, PresentsAnalytic presentsAnalytic, CompositeIdentificator compositeIdentificator, LinkedContent next = null)
		: base(analyticData, next)
	{
		Present = present;
		Quantity = quantity;
		CompositeIdentificator = compositeIdentificator;
		_presentsAnalytic = presentsAnalytic;
	}

	public override void AddCurrentToPlayer()
	{
		Present.AddCount(Quantity);
		_presentsAnalytic.SendReceivedEvent(Present.Id, Quantity, AnalyticData.SourceType);
		base.AddCurrentToPlayer();
	}

	public override Sprite GetIcon()
	{
		return Present.Icon;
	}

	public override Sprite GetProgressBarIcon()
	{
		throw new NotImplementedException();
	}

	public override Rarity GetRarity()
	{
		return Rarity.Rare;
	}

	public override string GetName()
	{
		return "present." + Present.Id;
	}

	public override string GetDescription()
	{
		return Quantity.ToString();
	}

	public override Map GetMap()
	{
		return new PresentMap(this);
	}

	public override LinkedContent Clone()
	{
		return new PresentLinkedContent(Present, Quantity, AnalyticData, _presentsAnalytic, CompositeIdentificator, base.CloneOfNext);
	}
}

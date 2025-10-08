using System;
using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Models;

public class ComingSoonDateLinkedContent : LinkedContent
{
	[Serializable]
	public class DateMap : Map
	{
		public int ID;

		public DateMap(ComingSoonDateLinkedContent source)
			: base(source)
		{
			ID = source._dateId;
		}
	}

	private const string LocalizationKey = "coming_soon_date.{0}";

	private readonly int _dateId;

	public override Type Type => typeof(ComingSoonDateLinkedContent);

	public ComingSoonDateLinkedContent(int dateId, LinkedContentAnalyticData analyticData, LinkedContent next = null)
		: base(analyticData, next)
	{
		_dateId = dateId;
	}

	public override Sprite GetIcon()
	{
		return null;
	}

	public override void AddCurrentToPlayer()
	{
	}

	public override Sprite GetProgressBarIcon()
	{
		return GetIcon();
	}

	public override Rarity GetRarity()
	{
		return Rarity.Common;
	}

	public override string GetName()
	{
		return $"coming_soon_date.{_dateId}";
	}

	public override string GetDescription()
	{
		return string.Empty;
	}

	public override Map GetMap()
	{
		return new DateMap(this);
	}

	public override LinkedContent Clone()
	{
		return new ComingSoonDateLinkedContent(_dateId, AnalyticData, base.CloneOfNext);
	}
}

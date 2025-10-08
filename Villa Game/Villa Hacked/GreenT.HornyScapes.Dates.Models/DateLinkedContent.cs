using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.Dates.Services;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Dates.Models;

public class DateLinkedContent : LinkedContent
{
	[Serializable]
	public class DateMap : Map
	{
		public int ID;

		public DateMap(DateLinkedContent source)
			: base(source)
		{
			ID = source._date.ID;
		}
	}

	private const string LocalizationKey = "date.{0}";

	private readonly Date _date;

	private readonly DateUnlockService _dateUnlockService;

	public Date Date => _date;

	public override Type Type => typeof(DateLinkedContent);

	public DateLinkedContent(Date date, DateUnlockService dateUnlockService, LinkedContentAnalyticData analyticData, LinkedContent next = null)
		: base(analyticData, next)
	{
		_date = date;
		_dateUnlockService = dateUnlockService;
	}

	public override Sprite GetIcon()
	{
		return _date.IconData.Icon;
	}

	public override void AddCurrentToPlayer()
	{
		_dateUnlockService.Unlock(_date);
		base.AddCurrentToPlayer();
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
		return $"date.{_date.ID}";
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
		return new DateLinkedContent(_date, _dateUnlockService, AnalyticData, base.CloneOfNext);
	}
}

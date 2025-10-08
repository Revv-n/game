using System;
using GreenT.Bonus;
using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Booster;

public class BoosterLinkedContent : LinkedContent
{
	[Serializable]
	public class BoosterMap : Map
	{
		[SerializeField]
		private int id;

		public int ID => id;

		public BoosterMap(BoosterLinkedContent source)
			: base(source)
		{
			id = source._id;
		}
	}

	private readonly int _id;

	private readonly GameSettings _settings;

	private readonly BoosterService _boosterService;

	public BonusType BonusType { get; }

	private BonusSettings _bonusSettings => _settings.BonusSettings[BonusType];

	public override Type Type => typeof(BoosterLinkedContent);

	public BoosterLinkedContent(int id, BonusType bonusType, BoosterService boosterService, GameSettings settings, LinkedContentAnalyticData analyticData, LinkedContent next = null)
		: base(analyticData, next)
	{
		_id = id;
		BonusType = bonusType;
		_boosterService = boosterService;
		_settings = settings;
	}

	public override void AddCurrentToPlayer()
	{
		base.AddCurrentToPlayer();
		_boosterService.ApplyBooster(_id);
	}

	public override Sprite GetIcon()
	{
		return _bonusSettings.BonusSprite;
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
		return _bonusSettings.BonusToolTipSettings.KeyName;
	}

	public override string GetDescription()
	{
		return _id.ToString();
	}

	public override Map GetMap()
	{
		return new BoosterMap(this);
	}

	public override LinkedContent Clone()
	{
		return new BoosterLinkedContent(_id, BonusType, _boosterService, _settings, AnalyticData);
	}
}

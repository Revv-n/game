using System;
using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Characters.Skins.Content;

public class SkinLinkedContent : LinkedContent
{
	[Serializable]
	public class SkinMap : Map
	{
		[SerializeField]
		private int id;

		public int ID => id;

		public SkinMap(SkinLinkedContent source)
			: base(source)
		{
			id = source.Skin.ID;
		}
	}

	private string skinNameKey;

	public Skin Skin { get; }

	public override Type Type => typeof(SkinLinkedContent);

	public SkinLinkedContent(Skin skin, LinkedContentAnalyticData analyticData, string skinNameKey, LinkedContent next = null)
		: base(analyticData, next)
	{
		Skin = skin;
		this.skinNameKey = skinNameKey;
	}

	public override LinkedContent Clone()
	{
		return new SkinLinkedContent(Skin, AnalyticData, skinNameKey, base.CloneOfNext);
	}

	public override string GetDescription()
	{
		return "1";
	}

	public override Sprite GetIcon()
	{
		return Skin.Data.Icon;
	}

	public override Sprite GetProgressBarIcon()
	{
		return Skin.Data.ProgressBarIcon;
	}

	public override Rarity GetRarity()
	{
		return Skin.Rarity;
	}

	public override Map GetMap()
	{
		return new SkinMap(this);
	}

	public override string GetName()
	{
		return skinNameKey;
	}

	public override void AddCurrentToPlayer()
	{
		Skin.Own();
		base.AddCurrentToPlayer();
	}

	public Sprite GetSquareIcon()
	{
		return Skin.Data.SquareIcon;
	}

	public Sprite GetFullIcon()
	{
		return Skin.Data.CardImage;
	}
}

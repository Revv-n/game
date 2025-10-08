using System;
using GreenT.HornyScapes.Analytics;
using GreenT.HornyScapes.MergeCore;
using Merge;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Content;

public class MergeItemLinkedContent : LinkedContent
{
	[Serializable]
	public class MergeItemMapper : Map
	{
		[SerializeField]
		private int id;

		[SerializeField]
		private int quantity;

		public int Id
		{
			get
			{
				return id;
			}
			private set
			{
				id = value;
			}
		}

		public int Quantity
		{
			get
			{
				return quantity;
			}
			private set
			{
				quantity = value;
			}
		}

		public MergeItemMapper(MergeItemLinkedContent source)
			: base(source)
		{
			Id = source.GameItemConfig.UniqId;
			Quantity = source.Quantity;
		}
	}

	private IMergeIconProvider _iconProvider;

	public int Quantity { get; }

	public GIConfig GameItemConfig { get; }

	public override Type Type => typeof(MergeItemLinkedContent);

	public MergeItemLinkedContent(IMergeIconProvider iconProvider, GIConfig gameItemConfig, int quantity, LinkedContentAnalyticData analyticData, LinkedContent next = null)
		: base(analyticData, next)
	{
		GameItemConfig = gameItemConfig;
		Quantity = quantity;
		_iconProvider = iconProvider;
	}

	public override Sprite GetIcon()
	{
		return _iconProvider.GetSprite(GameItemConfig.Key);
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
		return GameItemConfig.GameItemName;
	}

	public override string GetDescription()
	{
		return Quantity.ToString();
	}

	public override string ToString()
	{
		return GameItemConfig.GameItemName + " " + Quantity;
	}

	public override Map GetMap()
	{
		return new MergeItemMapper(this);
	}

	public override void AddCurrentToPlayer()
	{
		for (int i = 0; i != Quantity; i++)
		{
			GIData item = new GIData(GameItemConfig.Key);
			Controller<GreenT.HornyScapes.MergeCore.PocketController>.Instance.AddItemToQueue(item);
		}
		base.AddCurrentToPlayer();
	}

	public override LinkedContent Clone()
	{
		return new MergeItemLinkedContent(_iconProvider, GameItemConfig, Quantity, AnalyticData, base.CloneOfNext);
	}
}

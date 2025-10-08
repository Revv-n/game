using System;
using GreenT.HornyScapes.Analytics;
using StripClub.Model;
using StripClub.Model.Cards;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.Decorations;

public class DecorationLinkedContent : LinkedContent
{
	[Serializable]
	public class DecorationMap : Map
	{
		[SerializeField]
		private int _id;

		public int ID => _id;

		public DecorationMap(DecorationLinkedContent source)
			: base(source)
		{
			_id = source.ID;
		}
	}

	private const string key = "decoration.";

	private readonly DecorationController _decorationController;

	private readonly RoomManager _house;

	private readonly Decoration _decoration;

	public int ID { get; }

	public Decoration Decoration => _decoration;

	public override Type Type => typeof(DecorationLinkedContent);

	public DecorationLinkedContent(int id, RoomManager house, DecorationController decorationController, Decoration decoration, LinkedContentAnalyticData analyticData, LinkedContent next = null)
		: base(analyticData, next)
	{
		ID = id;
		_house = house;
		_decorationController = decorationController;
		_decoration = decoration;
	}

	public override Sprite GetIcon()
	{
		return _house.GetObject(_decoration.HouseObjectID).Config.CardViewIcon;
	}

	public override Sprite GetProgressBarIcon()
	{
		return _house.GetObject(_decoration.HouseObjectID).Config.PreviewIcon;
	}

	public override Rarity GetRarity()
	{
		return Rarity.Common;
	}

	public override string GetName()
	{
		return "decoration." + ID;
	}

	public override string GetDescription()
	{
		return "1";
	}

	public override LinkedContent Clone()
	{
		return new DecorationLinkedContent(ID, _house, _decorationController, _decoration, AnalyticData, base.CloneOfNext);
	}

	public override Map GetMap()
	{
		return new DecorationMap(this);
	}

	public override void AddCurrentToPlayer()
	{
		_decorationController.SetRewarded(_decoration);
		base.AddCurrentToPlayer();
	}
}

using System;
using StripClub.Model.Cards;
using UnityEngine;
using UnityEngine.Serialization;

namespace GreenT.HornyScapes.BannerSpace;

[Serializable]
public class RaskRarityInfo
{
	public Sprite Icon;

	[FormerlySerializedAs("RewardType")]
	public Rarity Rarity;
}

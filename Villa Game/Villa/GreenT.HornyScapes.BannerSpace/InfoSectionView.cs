using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Model.Cards;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class InfoSectionView : MonoView<RewardInfo[]>
{
	[Serializable]
	public class RarityInfo
	{
		public Rarity Rarity;

		public string LocalizationKey;

		public Color Color;
	}

	[SerializeField]
	private LocalizedTextMeshPro _name;

	[SerializeField]
	private RectTransform _container;

	[SerializeField]
	private List<RarityInfo> _rarityInfos = new List<RarityInfo>();

	private DropInfoCardViewManager _dropInfoCardViewManager;

	[Inject]
	public void Initialization(DropInfoCardViewManager dropInfoCardViewManager)
	{
		_dropInfoCardViewManager = dropInfoCardViewManager;
	}

	public override void Set(RewardInfo[] rewardInfos)
	{
		base.Set(rewardInfos);
		SetName(rewardInfos);
		RewardInfo[] array = (from rewardInfo in rewardInfos
			orderby rewardInfo.IsMain descending, rewardInfo.Chance
			select rewardInfo).ToArray();
		for (int i = 0; i < rewardInfos.Length; i++)
		{
			_dropInfoCardViewManager.Display(array[i], _container);
		}
	}

	private void SetName(RewardInfo[] rewardInfos)
	{
		Rarity rarity = rewardInfos.First().Rarity;
		RarityInfo rarityInfo = _rarityInfos.First((RarityInfo x) => x.Rarity == rarity);
		_name.Init(rarityInfo.LocalizationKey);
		_name.Text.color = rarityInfo.Color;
	}
}

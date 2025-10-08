using System;
using System.Linq;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.BannerSpace;

public class MainRewardInfoView : MonoBehaviour
{
	[Serializable]
	public class RarityInfo
	{
		public Rarity Rarity;

		public Color Color;
	}

	[Header("Main Reward")]
	[SerializeField]
	private RectTransform _mainRewardTextContainer;

	[SerializeField]
	private LocalizedTextMeshPro _mainRewardText;

	[SerializeField]
	private LocalizedTextMeshPro _mainRewardRarityText;

	[SerializeField]
	private RarityInfo[] _rarityInfos;

	public void Set(LinkedContent linkedContent, Vector2 anchoredPosition)
	{
		_mainRewardTextContainer.anchoredPosition = anchoredPosition;
		if (linkedContent != null)
		{
			_mainRewardText.Init(linkedContent.GetName());
			Rarity rarity = linkedContent.GetRarity();
			_mainRewardRarityText.Init("ui.rarity." + rarity.ToString().ToLower());
			_mainRewardRarityText.Text.color = _rarityInfos.FirstOrDefault((RarityInfo x) => x.Rarity == rarity)?.Color ?? Color.white;
		}
	}
}

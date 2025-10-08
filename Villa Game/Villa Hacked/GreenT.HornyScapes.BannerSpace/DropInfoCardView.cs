using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Characters;
using GreenT.HornyScapes.Characters.Skins.Content;
using StripClub.Model;
using StripClub.Model.Cards;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class DropInfoCardView : MonoView<RewardInfo>
{
	[Header("Other Info")]
	[SerializeField]
	private Image _newIcon;

	[SerializeField]
	private Image _isMainIcon;

	[Space(10f)]
	[Header("Rarity Info")]
	[SerializeField]
	private List<RaskRarityInfo> _rarityInfos;

	[SerializeField]
	private Image _rarityIcon;

	[Space(10f)]
	[Header("Items Info")]
	[SerializeField]
	private Image _itemIcon;

	[SerializeField]
	private Image _itemBackground;

	[SerializeField]
	private ItemBackgroundInfo _itemBackgroundInfo;

	[Space(10f)]
	[Header("Card Info")]
	[SerializeField]
	private Image _cardIcon;

	[Space(10f)]
	[Header("Chance Info")]
	[SerializeField]
	private TMP_Text _chanceText;

	private CharacterManager _characterManager;

	[Inject]
	private void Init(CharacterManager characterManager)
	{
		_characterManager = characterManager;
	}

	public override void Set(RewardInfo rewardInfo)
	{
		base.Set(rewardInfo);
		LinkedContent linkedContent = rewardInfo.LinkedContent;
		if (linkedContent is CardLinkedContent cardLinkedContent)
		{
			Rarity rarity = cardLinkedContent.GetRarity();
			_rarityIcon.sprite = _rarityInfos.FirstOrDefault((RaskRarityInfo x) => x.Rarity == rarity)?.Icon;
		}
		else if (linkedContent is SkinLinkedContent skinLinkedContent)
		{
			Rarity rarity = skinLinkedContent.GetRarity();
			_rarityIcon.sprite = _rarityInfos.FirstOrDefault((RaskRarityInfo x) => x.Rarity == rarity)?.Icon;
		}
		else
		{
			_rarityIcon.sprite = _rarityInfos.FirstOrDefault((RaskRarityInfo x) => x.Rarity == rewardInfo.Rarity)?.Icon;
		}
		_newIcon.gameObject.SetActive(rewardInfo.IsNew);
		_isMainIcon.gameObject.SetActive(rewardInfo.IsMain);
		_chanceText.text = rewardInfo.Chance + "%";
		SetIcon(rewardInfo);
	}

	private void SetIcon(RewardInfo rewardInfo)
	{
		LinkedContent linkedContent = rewardInfo.LinkedContent;
		Sprite icon = linkedContent.GetIcon();
		if (linkedContent is CardLinkedContent || linkedContent is SkinLinkedContent)
		{
			_itemBackground.gameObject.SetActive(value: false);
			_cardIcon.gameObject.SetActive(value: true);
			_cardIcon.sprite = icon;
		}
		else
		{
			_itemBackground.gameObject.SetActive(value: true);
			_cardIcon.gameObject.SetActive(value: false);
			_itemIcon.sprite = icon;
			_itemBackground.sprite = _itemBackgroundInfo.Get(linkedContent);
		}
	}
}

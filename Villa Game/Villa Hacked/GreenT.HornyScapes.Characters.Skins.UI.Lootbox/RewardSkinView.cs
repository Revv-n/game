using GreenT.AssetBundles;
using GreenT.HornyScapes.UI;
using GreenT.Localizations;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Characters.Skins.UI.Lootbox;

public class RewardSkinView : SkinView
{
	[SerializeField]
	protected Image skinImage;

	[SerializeField]
	protected CardPictureSelector pictureSelector;

	[SerializeField]
	protected TMP_Text objectName;

	[SerializeField]
	protected StatableComponent rarityStates;

	[SerializeField]
	private GameObject cardBack;

	private LocalizationService _localizationService;

	private FakeAssetService _fakeAssetService;

	[Inject]
	public void Init(LocalizationService localizationService, FakeAssetService fakeAssetService)
	{
		_localizationService = localizationService;
		_fakeAssetService = fakeAssetService;
	}

	public override void Set(Skin skin)
	{
		base.Set(skin);
		if (skin.IsDataEmpty)
		{
			_fakeAssetService.SetFakeSkinIcon(skin, skinImage, (Skin _) => skin.Data.CardImage);
		}
		else
		{
			skinImage.sprite = skin.Data.CardImage;
		}
		objectName.text = _localizationService.Text(skin.NameKey);
		rarityStates.Set((int)skin.Rarity);
		cardBack.SetActive(value: false);
	}
}

using GreenT.HornyScapes;
using GreenT.HornyScapes.Lootboxes;
using GreenT.Localizations;
using StripClub.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Rewards;

public class CardBattlePassLevelView : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI quantity;

	[SerializeField]
	private TextMeshProUGUI resourceName;

	[SerializeField]
	private TextMeshProUGUI subName;

	[SerializeField]
	private Image image;

	[SerializeField]
	private Image backplate;

	[SerializeField]
	private Image cardBack;

	private LocalizationService _localizationService;

	private GreenT.HornyScapes.GameSettings gameSettings;

	private RewSettings RewPlaceholder => gameSettings.RewPlaceholder[RewType.Level];

	[Inject]
	public void Init(LocalizationService localizationService, GreenT.HornyScapes.GameSettings gameSettings)
	{
		this.gameSettings = gameSettings;
		_localizationService = localizationService;
	}

	public void Set(BattlePassLevelLinkedContent content)
	{
		cardBack.gameObject.SetActive(value: false);
		resourceName.text = _localizationService.Text(RewPlaceholder.PremiumLocalizationKey);
		resourceName.color = RewPlaceholder.PremiumColor;
		subName.text = _localizationService.Text(RewPlaceholder.PremiumSubNameLocalizationKey);
		image.sprite = content.GetIcon();
		backplate.sprite = RewPlaceholder.LevelRewardBackplate;
		if (content.Quantity > 0)
		{
			TextMeshProUGUI textMeshProUGUI = quantity;
			int num = content.Quantity;
			textMeshProUGUI.text = "+" + num + " " + _localizationService.Text(RewPlaceholder.LevelLocalizationKey);
		}
		else
		{
			quantity.text = "";
		}
	}
}

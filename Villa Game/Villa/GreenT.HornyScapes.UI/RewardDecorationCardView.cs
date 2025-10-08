using GreenT.HornyScapes.Meta.Decorations;
using GreenT.Localizations;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI;

public class RewardDecorationCardView : MonoView<DecorationLinkedContent>
{
	private const string LocalizationKey = "decoration.";

	[SerializeField]
	private Image fx;

	[SerializeField]
	private TextMeshProUGUI itemName;

	[SerializeField]
	private Image item;

	[SerializeField]
	private GameObject card;

	[SerializeField]
	private GameObject cardBack;

	private LocalizationService _localizationService;

	[Inject]
	public void Init(LocalizationService localizationService)
	{
		_localizationService = localizationService;
	}

	public override void Set(DecorationLinkedContent content)
	{
		base.Set(content);
		fx.gameObject.SetActive(value: true);
		base.gameObject.SetActive(value: true);
		card.SetActive(value: true);
		SetCardBack(state: false);
		item.sprite = content.GetIcon();
		itemName.text = _localizationService.Text("decoration." + content.ID);
	}

	public void SetCardBack(bool state)
	{
		cardBack.SetActive(state);
	}
}

using System;
using GreenT.Bonus;
using GreenT.HornyScapes;
using GreenT.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Rewards;

public class CardBoosterView : MonoBehaviour
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

	private GreenT.HornyScapes.GameSettings _gameSettings;

	private LocalizationService _localization;

	private IDisposable _iconChangeStream;

	[Inject]
	public void Construct(GreenT.HornyScapes.GameSettings gameSettings, LocalizationService localization)
	{
		_gameSettings = gameSettings;
		_localization = localization;
	}

	public void Set(BonusType bonusType)
	{
		BonusSettings bonusSettings = _gameSettings.BonusSettings[bonusType];
		CardBackView(showState: false);
		string keyName = bonusSettings.BonusToolTipSettings.KeyName;
		resourceName.text = _localization.Text(keyName);
		image.sprite = bonusSettings.BonusSprite;
	}

	public void SetActiveQuantity(bool state)
	{
		quantity.gameObject.SetActive(state);
	}

	public void CardBackView(bool showState)
	{
		cardBack.gameObject.SetActive(showState);
	}

	private void OnDestroy()
	{
		_iconChangeStream?.Dispose();
	}
}

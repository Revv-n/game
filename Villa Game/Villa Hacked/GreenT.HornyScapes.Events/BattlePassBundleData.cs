using UnityEngine;

namespace GreenT.HornyScapes.Events;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/BattlePass/Bundle")]
public class BattlePassBundleData : ScriptableObject, IBundleData
{
	public string Type;

	[Header("Локализация")]
	public string TitleKeyLoc = "ui.battlepass.progresswindow.title";

	public string DescriptionKeyLoc = "ui.battlepass.startwindow.description";

	public string CurrencyKeyLoc = "ui.battlepass.currency";

	public string ActivatePremium = "ui.battlepass.target";

	public string PremiumLevelBonus;

	[Header("Банк")]
	[Tooltip("Иконка валюты пропуска")]
	public Sprite Currency;

	[Tooltip("Подложка под покупку двух премиумов")]
	public Sprite DoublePurchaseWindow;

	[Tooltip("Подложка под покупку одного премиума")]
	public Sprite SinglePurchaseWindow;

	[Header("Уровень")]
	[Tooltip("Иконка под уровнем пропуска")]
	public Sprite LevelHolder;

	[Tooltip("Иконка статуса пропуска")]
	public Sprite ButtonSp;

	[Header("Окно наград")]
	public Sprite PremiumTrackArrow;

	public Sprite ProgressGirl;

	public Sprite HeaderImage;

	public Sprite ActivationBackground;

	public Sprite ActivatedBackground;

	public Sprite MergedRewardHolder;

	[Header("Стартовое окно")]
	public Sprite LeftGirl;

	public Sprite RightGirl;

	public Sprite RewardPreview;

	public Sprite StartWindowBackground;

	[Header("Окно покупки")]
	public Sprite LeftReward;

	public Sprite RightReward;

	public Sprite LevelBonusHolder;

	[Header("Покупка уровней и према")]
	public Sprite LevelRewardBaseIcon;

	public Sprite LevelRewardExpensiveIcon;

	public Sprite LevelRewardBackplate;

	public string LevelLocalizationKey;

	public string PremiumLocalizationKey;

	public string PremiumSubNameLocalizationKey;

	public Color PremiumColor;

	[field: SerializeField]
	public Sprite AnnouncementBackground { get; private set; }

	[field: SerializeField]
	public Sprite AnnouncementTitleBackground { get; private set; }
}

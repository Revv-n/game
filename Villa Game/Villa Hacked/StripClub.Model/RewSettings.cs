using System;
using GreenT.HornyScapes.Events;
using UnityEngine;

namespace StripClub.Model;

[Serializable]
public class RewSettings
{
	[SerializeField]
	private Sprite levelRewardBaseIcon;

	[SerializeField]
	private Sprite levelRewardExpensiveIcon;

	[SerializeField]
	private Sprite levelRewardBackplate;

	[SerializeField]
	private string levelLocalizationKey;

	[SerializeField]
	private string premiumLocalizationKey;

	[SerializeField]
	private string premiumSubNameLocalizationKey;

	[SerializeField]
	private Color premiumColor;

	public Sprite LevelRewardBaseIcon => levelRewardBaseIcon;

	public Sprite LevelRewardExpensiveIcon => levelRewardExpensiveIcon;

	public Sprite LevelRewardBackplate => levelRewardBackplate;

	public string LevelLocalizationKey => levelLocalizationKey;

	public string PremiumLocalizationKey => premiumLocalizationKey;

	public string PremiumSubNameLocalizationKey => premiumSubNameLocalizationKey;

	public Color PremiumColor => premiumColor;

	public void Set(BattlePassBundleData bundle)
	{
		levelRewardBaseIcon = bundle.LevelRewardBaseIcon;
		levelRewardExpensiveIcon = bundle.LevelRewardExpensiveIcon;
		levelRewardBackplate = bundle.LevelRewardBackplate;
		levelLocalizationKey = bundle.LevelLocalizationKey;
		premiumLocalizationKey = bundle.PremiumLocalizationKey;
		premiumSubNameLocalizationKey = bundle.PremiumSubNameLocalizationKey;
		premiumColor = bundle.PremiumColor;
	}
}

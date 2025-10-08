using System;
using System.Text.RegularExpressions;
using GreenT.HornyScapes;
using GreenT.HornyScapes.Card.Bonus;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.GameItems;
using GreenT.HornyScapes.MergeCore;
using GreenT.HornyScapes.ToolTips;
using GreenT.Localizations;
using Merge;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Collections.Promote;

public class NewGirlBonusView : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI spawnerName;

	[SerializeField]
	private TextMeshProUGUI bonusInfo;

	[SerializeField]
	private TextMeshProUGUI bonusValue;

	[SerializeField]
	private TextMeshProUGUI bonusValueMax;

	[SerializeField]
	private Image bonusIcon;

	[SerializeField]
	private Image affectedItemIcon;

	[SerializeField]
	private ToolTipUIOpener bonusTTOpener;

	[SerializeField]
	private Sprite allAffected;

	[Inject]
	private GreenT.HornyScapes.GameSettings gameSettings;

	private IMergeIconProvider _mergeIconProvider;

	private LocalizationService _localization;

	private GameItemConfigManager _gameItemConfigManager;

	private IDisposable _nameDisposable;

	private IDisposable _bonusDisposable;

	[Inject]
	private void Init(IMergeIconProvider mergeIconManager, LocalizationService localization, GameItemConfigManager gameItemConfigManager)
	{
		_mergeIconProvider = mergeIconManager;
		_localization = localization;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public void Init(CharacterMultiplierBonus bonus)
	{
		_nameDisposable?.Dispose();
		_bonusDisposable?.Dispose();
		string patternLong = "<color=[^>]+>\\s*\\{0\\}\\s*</color> ";
		string patternShort = "\\s*\\{0\\}\\s* ";
		_localization.Text(bonus.SpawnerName);
		_localization.Text(bonus.Name);
		_nameDisposable = ObservableExtensions.Subscribe<string>((IObservable<string>)_localization.ObservableText(bonus.SpawnerName), (Action<string>)delegate(string text)
		{
			spawnerName.text = char.ToUpper(text[0]) + text.Substring(1);
		});
		_bonusDisposable = ObservableExtensions.Subscribe<string>((IObservable<string>)_localization.ObservableText(bonus.Name), (Action<string>)delegate(string text)
		{
			string input = text;
			input = Regex.Replace(input, patternLong, "").Trim();
			input = Regex.Replace(input, patternShort, "").Trim();
			bonusInfo.text = input;
		});
		affectedItemIcon.sprite = (bonus.AffectAll ? allAffected : GetMaxOpenedSpawnerSprite(bonus));
		bonusValue.text = bonus.ToString(Math.Max(1, bonus.Level));
		bonusValueMax.text = bonus.ToString(bonus.Values.Length - 1);
		bonusIcon.sprite = gameSettings.BonusSettings[bonus.BonusType].BonusSprite;
		bonusTTOpener.Init(gameSettings.BonusSettings[bonus.BonusType].BonusToolTipSettings);
	}

	public Sprite GetMaxOpenedSpawnerSprite(CharacterMultiplierBonus bonus)
	{
		GIKey spriteMaxOpenedSpawner = BonusTools.GetSpriteMaxOpenedSpawner(_gameItemConfigManager, bonus);
		return _mergeIconProvider.GetSprite(spriteMaxOpenedSpawner);
	}

	private void OnDisable()
	{
		_nameDisposable?.Dispose();
		_bonusDisposable?.Dispose();
	}
}

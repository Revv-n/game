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

public class BonusView : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI bonusShortDescription;

	[SerializeField]
	private Image[] bonusIcons;

	[SerializeField]
	private TextMeshProUGUI bonusValue;

	[SerializeField]
	private TextMeshProUGUI bonusNextValue;

	[SerializeField]
	private GameObject arrow;

	[SerializeField]
	private Image affectedItemIcon;

	[SerializeField]
	private ToolTipUIOpener[] bonusTTOpeners;

	[SerializeField]
	private Sprite allAffected;

	[Inject]
	private GreenT.HornyScapes.GameSettings gameSettings;

	private IMergeIconProvider _mergeIconManager;

	private LocalizationService _localizationService;

	private GameItemConfigManager _gameItemConfigManager;

	private CompositeDisposable _localizationDisposables = new CompositeDisposable();

	[Inject]
	private void Init(IMergeIconProvider mergeIconManager, LocalizationService localizationService, GameItemConfigManager gameItemConfigManager)
	{
		_mergeIconManager = mergeIconManager;
		_localizationService = localizationService;
		_gameItemConfigManager = gameItemConfigManager;
	}

	public void Init(CharacterMultiplierBonus bonus)
	{
		affectedItemIcon.sprite = (bonus.AffectAll ? allAffected : GetMaxOpenedSpawnerSprite(bonus));
		_localizationDisposables.Clear();
		IReadOnlyReactiveProperty<string> left = _localizationService.ObservableText(bonus.Name);
		IReadOnlyReactiveProperty<string> right = _localizationService.ObservableText(bonus.SpawnerName);
		left.CombineLatest(right, (string name, string spawnerName) => string.Format(name, spawnerName)).Subscribe(delegate(string formatted)
		{
			bonusShortDescription.text = formatted;
		}).AddTo(_localizationDisposables);
		bonusValue.text = bonus.ToString();
		Image[] array = bonusIcons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].sprite = gameSettings.BonusSettings[bonus.BonusType].BonusSprite;
		}
		ToolTipUIOpener[] array2 = bonusTTOpeners;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Init(gameSettings.BonusSettings[bonus.BonusType].BonusToolTipSettings);
		}
		if (bonus.Level + 1 < bonus.Values.Length)
		{
			arrow.SetActive(value: true);
			bonusNextValue.text = bonus.ToString(bonus.Level + 1);
		}
		else
		{
			arrow.SetActive(value: false);
			bonusNextValue.text = string.Empty;
		}
	}

	public Sprite GetMaxOpenedSpawnerSprite(CharacterMultiplierBonus bonus)
	{
		GIKey spriteMaxOpenedSpawner = BonusTools.GetSpriteMaxOpenedSpawner(_gameItemConfigManager, bonus);
		return _mergeIconManager.GetSprite(spriteMaxOpenedSpawner);
	}

	private void OnDisable()
	{
		_localizationDisposables?.Clear();
	}

	private void OnDestroy()
	{
		_localizationDisposables?.Dispose();
	}
}

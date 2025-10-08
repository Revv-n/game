using System;
using GreenT.Bonus;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Booster;

public class BoosterDropView : MonoView
{
	public class Manager : ViewManager<BoosterDropView>
	{
	}

	private const string LocalizationKey = "ui.booster.{0}";

	[SerializeField]
	private Image _icon;

	[SerializeField]
	private SpriteStates _valueInfoStates;

	[SerializeField]
	private TMP_Text _valueField;

	[SerializeField]
	private LocalizedTextMeshPro _infoField;

	private GameSettings _gameSettings;

	private IDisposable _iconChangeStream;

	[Inject]
	private void Construct(GameSettings gameSettings)
	{
		_gameSettings = gameSettings;
	}

	public void Set(BonusType bonusType, int value)
	{
		if (!_gameSettings.BonusSettings.TryGetValue(bonusType, out var value2))
		{
			Debug.LogException(new ArgumentOutOfRangeException($"Dictionary [{_gameSettings.BonusSettings.GetType()} missing type {bonusType}]"));
			return;
		}
		_icon.sprite = value2.BonusSprite;
		_valueInfoStates.Set((int)bonusType);
		string formattedValue = GetFormattedValue(bonusType, value);
		_valueField.text = formattedValue;
		if (!(_infoField == null))
		{
			string key = $"ui.booster.{(int)bonusType}";
			_infoField.Init(key, formattedValue);
		}
	}

	private string GetFormattedValue(BonusType bonusType, int value)
	{
		string text = ((bonusType != BonusType.IncreaseEnergyRechargeSpeed) ? string.Empty : "x");
		string text2 = text;
		text = ((bonusType != BonusType.FreeSummon) ? value.ToString() : TimeSpan.FromSeconds(value).TotalHours.ToString());
		string text3 = text;
		text = ((bonusType != BonusType.FreeSummon) ? string.Empty : "h");
		string text4 = text;
		return text2 + text3 + text4;
	}
}

using System;
using StripClub.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Cheats.Balance;

public class CurrencyBalanceElement : MonoBehaviour
{
	[SerializeField]
	private Image _icon;

	[SerializeField]
	private TMP_Text _addValue;

	[SerializeField]
	private TMP_Text _spendValue;

	private CurrencyType currencyType;

	private double addValue;

	private double spendValue;

	public CurrencyType CurrencyType => currencyType;

	public void Init(Sprite sprite, CurrencyType currencyType)
	{
		this.currencyType = currencyType;
		addValue = 0.0;
		spendValue = 0.0;
		_icon.sprite = sprite;
		_addValue.text = $"+{addValue}";
		_spendValue.text = $"-{spendValue}";
	}

	public void UpdateSprite(Sprite sprite)
	{
		_icon.sprite = sprite;
	}

	public void DefaultValue()
	{
		addValue = 0.0;
		spendValue = 0.0;
		_addValue.text = $"+{addValue}";
		_spendValue.text = $"-{spendValue}";
	}

	public void SetValue(double value)
	{
		if (value >= 0.0)
		{
			addValue += value;
			_addValue.text = $"+{addValue}";
		}
		else
		{
			spendValue += Math.Abs(value);
			_spendValue.text = $"-{spendValue}";
		}
	}
}

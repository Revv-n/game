using System;
using GreenT.HornyScapes;
using StripClub.Model.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class PriceView : MonoBehaviour, IView
{
	[SerializeField]
	private TextMeshProUGUI priceValue;

	[SerializeField]
	private Image currencyIcon;

	[SerializeField]
	private TextMeshProUGUI currencySign;

	private GreenT.HornyScapes.GameSettings settings;

	public int SiblingIndex
	{
		get
		{
			return base.transform.GetSiblingIndex();
		}
		set
		{
			base.transform.SetSiblingIndex(value);
		}
	}

	[Inject]
	public void Init(GreenT.HornyScapes.GameSettings settings)
	{
		this.settings = settings;
	}

	public void Set<T>(Price<T> price) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		priceValue.text = price.ToString();
		if (settings.CurrencySettings.TryGetValue(price.Currency, out var currencySettings, price.CompositeIdentificator))
		{
			currencyIcon.sprite = currencySettings.Sprite;
			currencyIcon.gameObject.SetActive(value: true);
		}
		else
		{
			currencyIcon.gameObject.SetActive(value: false);
		}
	}

	public void Display(bool isOn)
	{
		base.gameObject.SetActive(isOn);
	}

	public bool IsActive()
	{
		return base.gameObject.activeSelf;
	}
}

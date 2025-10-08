using System;
using System.Collections.Generic;
using GreenT.HornyScapes;
using GreenT.Types;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace StripClub.UI.Shop;

public class PriceWithFreeView : MonoBehaviour, IView
{
	[SerializeField]
	private TextMeshProUGUI priceValue;

	[SerializeField]
	private Image currencyIcon;

	[SerializeField]
	private GameObject[] priceObjects;

	[SerializeField]
	private GameObject[] freeLotOjbect;

	[SerializeField]
	private GameObject[] attentionIconsObjects;

	[SerializeField]
	private List<StatableComponent> statableComponents;

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
		Set(price.Value, price.Currency, price.CompositeIdentificator, price.Equals(Price<T>.Free));
	}

	public void Set<T>(Price<T> price, bool isAttentionIconsOn) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		Set(price.Value, price.Currency, price.CompositeIdentificator, price.Equals(Price<T>.Free), isAttentionIconsOn);
	}

	public void Set<T>(T value, CurrencyType currencyType, CompositeIdentificator compositeIdentificator = default(CompositeIdentificator), bool isFree = false, bool isAttentionIconsOn = false) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		bool flag = isFree;
		GameObject[] array = priceObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
		array = freeLotOjbect;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		array = attentionIconsObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(isAttentionIconsOn);
		}
		if (!flag)
		{
			priceValue.text = value.ToString();
			if (settings.CurrencySettings.TryGetValue(currencyType, out var currencySettings, compositeIdentificator))
			{
				currencyIcon.sprite = currencySettings.Sprite;
				currencyIcon.gameObject.SetActive(value: true);
			}
			else
			{
				currencyIcon.gameObject.SetActive(value: false);
			}
		}
	}

	public void SetValueColor(int index)
	{
		foreach (StatableComponent statableComponent in statableComponents)
		{
			statableComponent.Set(index);
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

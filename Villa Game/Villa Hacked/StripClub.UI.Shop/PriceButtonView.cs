using System;
using StripClub.Model.Shop;
using UnityEngine;

namespace StripClub.UI.Shop;

public class PriceButtonView : MonoBehaviour
{
	[SerializeField]
	private PriceView oldPriceView;

	[SerializeField]
	private PriceView currentPriceView;

	[SerializeField]
	private GameObject[] noPriceObject;

	[SerializeField]
	private GameObject[] withPriceObjects;

	[SerializeField]
	private GameObject[] oldPriceObjects;

	public void Set<T, K>(Price<T> price, Price<K> oldPrice) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable where K : struct, IComparable, IComparable<K>, IConvertible, IEquatable<K>, IFormattable
	{
		Set(price);
		if (oldPriceView != null)
		{
			if (oldPrice != null)
			{
				oldPriceView.Set(oldPrice);
			}
			GameObject[] array = oldPriceObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(oldPrice != null);
			}
		}
	}

	public void Set<T>(Price<T> price) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		currentPriceView.Set(price);
		bool flag = price.Equals(Price<T>.Free);
		GameObject[] array = noPriceObject;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		array = withPriceObjects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(!flag);
		}
	}
}

using System;
using StripClub.Model;
using StripClub.Model.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Merge.Core.Inventory;

public class InventoryWindowBuyButton : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI priceLabel;

	[SerializeField]
	private Button button;

	[SerializeField]
	private CurrencyType priceType;

	private ICurrencyProcessor currencyProcessor;

	public int Price { get; private set; }

	public event Action OnClick;

	public void Init(ICurrencyProcessor currencyProcessor)
	{
		this.currencyProcessor = currencyProcessor;
	}

	public void SetPrice(int price)
	{
		Price = price;
		priceLabel.text = price.ToString();
	}

	private void Start()
	{
		button.AddClickCallback(delegate
		{
			this.OnClick();
		});
	}
}

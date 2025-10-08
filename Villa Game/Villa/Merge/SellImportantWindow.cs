using System;
using Merge.Core.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace Merge;

public class SellImportantWindow : PopupWindow
{
	[SerializeField]
	private Image iconImage;

	[SerializeField]
	private Text priceLabel;

	[SerializeField]
	private Button sellBtn;

	private GameItem cacedGameItem;

	public event Action<GameItem> OnSellClick;

	private void Start()
	{
	}

	public SellImportantWindow SetValue(GameItem gi)
	{
		cacedGameItem = gi;
		AtSellClick();
		return this;
	}

	private void AtSellClick()
	{
		this.OnSellClick?.Invoke(cacedGameItem);
	}
}

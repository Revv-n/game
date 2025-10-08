using System;
using UnityEngine;

namespace Merge;

[Serializable]
public struct GameItemMergePointsCase
{
	[SerializeField]
	private SpriteRenderer battlePassCurrencyIcon;

	public void Activate(Sprite icon)
	{
		battlePassCurrencyIcon.sprite = icon;
		battlePassCurrencyIcon.gameObject.SetActive(value: true);
	}

	public void Deactivate()
	{
		battlePassCurrencyIcon.gameObject.SetActive(value: false);
		battlePassCurrencyIcon.sprite = null;
	}
}

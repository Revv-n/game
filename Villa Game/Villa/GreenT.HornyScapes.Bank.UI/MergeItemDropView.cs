using Merge;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Bank.UI;

public class MergeItemDropView : MonoView
{
	public class Manager : ViewManager<MergeItemDropView>
	{
	}

	[SerializeField]
	private Image icon;

	[SerializeField]
	private TMP_Text count;

	public GIConfig Source { get; private set; }

	public void Set(GIConfig source, Sprite itemIcon, int? quantity)
	{
		Source = source;
		icon.sprite = itemIcon;
		SetQuantity(quantity);
	}

	private void SetQuantity(int? quantity)
	{
		if (quantity.HasValue)
		{
			count.SetActive(active: true);
			count.text = quantity.Value.ToString();
		}
		else
		{
			count.SetActive(active: false);
			count.text = string.Empty;
		}
	}
}

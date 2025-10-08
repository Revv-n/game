using GreenT.HornyScapes.Characters;
using Merge;
using StripClub.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Model.Shop.UI;

public class CardDropView : MonoView
{
	public class Manager : ViewManager<CardDropView>
	{
	}

	public Image icon;

	[SerializeField]
	private TMP_Text count;

	[SerializeField]
	private StatableComponent[] statables;

	public ICharacter Source { get; private set; }

	public void SetCharacter(ICharacter character, int? quantity, int rarity)
	{
		Source = character;
		SetQuantity(quantity);
		SetStatables(rarity);
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

	private void SetStatables(int rarity)
	{
		StatableComponent[] array = statables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Set(rarity);
		}
	}
}

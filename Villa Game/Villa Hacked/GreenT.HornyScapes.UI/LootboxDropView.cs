using GreenT.HornyScapes.Lootboxes;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.UI;

public sealed class LootboxDropView : MonoView
{
	public class Manager : ViewManager<LootboxDropView>
	{
	}

	[SerializeField]
	private StatableComponent statable;

	public Lootbox Lootbox;

	public void Set(Lootbox lootbox, int? rarity)
	{
		Lootbox = lootbox;
		SetRarity(rarity);
	}

	private void SetRarity(int? rarity)
	{
		if (rarity.HasValue)
		{
			statable.Set(rarity.Value);
		}
		else
		{
			statable.Set((int)Lootbox.Rarity);
		}
	}
}

using GreenT.HornyScapes.Meta.RoomObjects;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Bank.UI;

public class DecorationDropViewWithRarity : DecorationDropView
{
	public new class Manager : ViewManager<DecorationDropViewWithRarity>
	{
	}

	[SerializeField]
	private StatableComponent statable;

	public void Set(BaseObjectConfig config, int rarity)
	{
		Set(config);
		statable.Set(rarity);
	}
}

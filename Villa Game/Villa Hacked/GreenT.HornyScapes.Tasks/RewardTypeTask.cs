using StripClub.Model;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks;

public class RewardTypeTask : MonoView<LinkedContent>
{
	private const int RESOURCES_BG = 0;

	private const int LOOTBOX_BG = 1;

	[Header("Uncomplete:")]
	[SerializeField]
	private StatableComponent bgIcon;

	[SerializeField]
	private TaskRewardIconsSetter icon;

	[SerializeField]
	private StatableComponent iconLootbox;

	[Header("Complete:")]
	[SerializeField]
	private TaskRewardIconsSetter iconComplete;

	[SerializeField]
	private StatableComponent iconLootboxComplete;

	public override void Set(LinkedContent source)
	{
		base.Set(source);
		icon.gameObject.SetActive(value: false);
		iconComplete.gameObject.SetActive(value: false);
		iconLootbox.gameObject.SetActive(value: false);
		iconLootboxComplete.gameObject.SetActive(value: false);
		if (!(source is CurrencyLinkedContent linkedContent))
		{
			if (source is LootboxLinkedContent lootboxLinkedContent)
			{
				iconLootbox.gameObject.SetActive(value: true);
				iconLootboxComplete.gameObject.SetActive(value: true);
				iconLootbox.Set((int)lootboxLinkedContent.Lootbox.Rarity);
				iconLootboxComplete.Set((int)lootboxLinkedContent.Lootbox.Rarity);
				bgIcon.Set(1);
			}
		}
		else
		{
			icon.gameObject.SetActive(value: true);
			iconComplete.gameObject.SetActive(value: true);
			icon.Set(linkedContent);
			iconComplete.Set(linkedContent);
			bgIcon.Set(0);
		}
	}
}

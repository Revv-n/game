using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "Prestige Item", menuName = "StripClub/Items/Prestige Item", order = 5)]
public class PrestigeItemInfo : NamedScriptableItemInfo
{
	private const string prefix = "content.items.prestige.";

	[SerializeField]
	protected Sprite roomSprite;

	protected override string GetKey()
	{
		return "content.items.prestige." + key;
	}
}

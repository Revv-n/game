using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "Item", menuName = "StripClub/Items/Card Item", order = 6)]
public class CardItemInfo : NamedScriptableItemInfo
{
	private const string prefix = "content.items.promote.";

	[field: SerializeField]
	public int ConfigID { get; private set; }

	protected override string GetKey()
	{
		return "content.items.promote." + ConfigID;
	}
}

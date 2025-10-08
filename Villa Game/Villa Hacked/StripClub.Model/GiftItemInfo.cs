using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "Gift", menuName = "StripClub/Items/Gift", order = 0)]
public class GiftItemInfo : NamedScriptableItemInfo
{
	private const string prefix = "content.items.gift.";

	[field: SerializeField]
	public int ConfigID { get; private set; } = -1;


	protected override string GetKey()
	{
		return "content.items.gift." + ConfigID;
	}
}

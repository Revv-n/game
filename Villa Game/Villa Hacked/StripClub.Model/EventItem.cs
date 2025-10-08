using UnityEngine;

namespace StripClub.Model;

[CreateAssetMenu(fileName = "EventItem", menuName = "StripClub/Items/Event Item", order = 6)]
public class EventItem : NamedScriptableItemInfo
{
	[SerializeField]
	private int event_id;

	private const string prefix = "content.items.event.";

	public int EventID => event_id;

	protected override string GetKey()
	{
		return "content.items.event." + key;
	}
}

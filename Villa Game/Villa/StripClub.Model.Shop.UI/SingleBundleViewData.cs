using UnityEngine;

namespace StripClub.Model.Shop.UI;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/Bank/Bundle/Single View Data")]
public class SingleBundleViewData : ScriptableObject
{
	[field: SerializeField]
	public Sprite Girl { get; private set; }

	[field: SerializeField]
	public Sprite Background { get; private set; }
}

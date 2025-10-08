using UnityEngine;

namespace GreenT.HornyScapes.Bank;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/Bank/Offer/Preview Preset")]
public class ViewPreset : ScriptableObject
{
	[field: SerializeField]
	public RegionSpriteDictionary Icon { get; set; }

	[field: SerializeField]
	public OfferPreviewBackgroundPreset Background { get; set; }
}

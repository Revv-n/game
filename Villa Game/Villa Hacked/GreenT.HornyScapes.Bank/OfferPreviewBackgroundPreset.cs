using UnityEngine;

namespace GreenT.HornyScapes.Bank;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/Bank/Offer/Preview Background Preset")]
public class OfferPreviewBackgroundPreset : ScriptableObject
{
	[field: SerializeField]
	public RegionSpriteDictionary Sprite { get; set; }

	public Sprite this[OfferPreviewRegion region] => Sprite[region];
}

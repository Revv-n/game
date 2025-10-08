using TMPro;
using UnityEngine;

namespace StripClub.Model.Shop.Data;

[CreateAssetMenu(menuName = "GreenT/HornyScapes/Bank/Bundle/List View Data")]
public class ListBundleViewData : ScriptableObject
{
	[field: SerializeField]
	public Sprite Girl { get; private set; }

	[field: SerializeField]
	public Sprite Background { get; private set; }

	[field: SerializeField]
	public TMP_ColorGradient ColoGradient { get; private set; }

	[field: SerializeField]
	public Material TextMaterialPreset { get; private set; }
}

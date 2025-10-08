using System;
using TMPro;
using UnityEngine;

namespace StripClub.UI.Shop;

[Serializable]
[CreateAssetMenu(menuName = "StripClub/Shop/Slots/Settings")]
public class SlotsSectionSettings : ScriptableObject
{
	[field: SerializeField]
	public string LocalizationKey { get; private set; } = "ui.shop.slots.header.";


	[field: SerializeField]
	public Sprite Header { get; private set; }

	[field: SerializeField]
	public Sprite DoubleValue { get; private set; }

	[field: SerializeField]
	public Sprite ExtraReward { get; private set; }

	[field: SerializeField]
	public Sprite LotBackground { get; private set; }

	[field: SerializeField]
	public Color TitleColor { get; private set; } = Color.black;


	[field: SerializeField]
	public TMP_ColorGradient TitleColorGradient { get; private set; }
}

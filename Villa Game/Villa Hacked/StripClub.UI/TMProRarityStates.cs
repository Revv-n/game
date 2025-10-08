using StripClub.Model.Cards;
using TMPro;
using UnityEngine;

namespace StripClub.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProRarityStates : StatableComponent
{
	[SerializeField]
	private TextMeshProUGUI text;

	private void OnValidate()
	{
		if (!text)
		{
			text = GetComponent<TextMeshProUGUI>();
		}
	}

	public override void Set(int stateNumber)
	{
		Rarity rarity = (Rarity)stateNumber;
		text.text = rarity.ToString();
	}
}

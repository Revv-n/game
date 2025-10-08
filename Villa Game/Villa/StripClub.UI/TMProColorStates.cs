using TMPro;
using UnityEngine;

namespace StripClub.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMProColorStates : StatableComponent
{
	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private IntColorDictionary textColors = new IntColorDictionary();

	private void OnValidate()
	{
		if (text == null)
		{
			text = GetComponent<TextMeshProUGUI>();
		}
	}

	public override void Set(int stateNumber)
	{
		text.color = textColors[stateNumber];
	}
}

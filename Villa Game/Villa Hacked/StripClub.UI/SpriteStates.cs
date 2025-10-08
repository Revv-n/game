using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI;

public class SpriteStates : StatableComponent
{
	[SerializeField]
	private IntSpriteDictionary states;

	[SerializeField]
	private Image image;

	private void OnValidate()
	{
		if (image == null)
		{
			image = GetComponent<Image>();
		}
	}

	public override void Set(int stateNumber)
	{
		if (states.Keys.Contains(stateNumber))
		{
			image.sprite = states[stateNumber];
		}
	}
}

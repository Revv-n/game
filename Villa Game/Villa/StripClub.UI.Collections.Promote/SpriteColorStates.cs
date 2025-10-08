using UnityEngine;
using UnityEngine.UI;

namespace StripClub.UI.Collections.Promote;

[RequireComponent(typeof(Image))]
public class SpriteColorStates : StatableComponent
{
	[SerializeField]
	private IntColorDictionary states;

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
		image.color = states[stateNumber];
	}
}

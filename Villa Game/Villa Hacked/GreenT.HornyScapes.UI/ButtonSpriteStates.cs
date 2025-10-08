using System;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.UI;

public class ButtonSpriteStates : StatableComponent
{
	[Serializable]
	internal struct ButtonSettings
	{
		[field: SerializeField]
		public Sprite @default { get; private set; }

		[field: SerializeField]
		public SpriteState spriteState { get; private set; }
	}

	[SerializeField]
	private Button button;

	[SerializeField]
	private ButtonSettings[] settings;

	public override void Set(int stateNumber)
	{
		button.spriteState = settings[stateNumber].spriteState;
		button.image.sprite = settings[stateNumber].@default;
	}

	private void OnValidate()
	{
		if (button == null)
		{
			button = GetComponent<Button>();
		}
	}
}

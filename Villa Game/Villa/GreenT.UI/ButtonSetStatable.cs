using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.UI;

public class ButtonSetStatable : StatableComponent
{
	[SerializeField]
	private Button _button;

	public override void Set(int stateNumber)
	{
		_button.interactable = stateNumber != 0;
	}
}

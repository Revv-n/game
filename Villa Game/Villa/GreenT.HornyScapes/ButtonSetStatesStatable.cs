using StripClub.UI;
using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class ButtonSetStatesStatable : StatableComponentArray<Button, SpriteState>
{
	public override void Set(int stateNumber)
	{
		element.spriteState = states[stateNumber];
	}
}

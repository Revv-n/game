using UnityEngine;

namespace GreenT.HornyScapes;

public class RendererColorStates : ColorStatableComponent<SpriteRenderer>
{
	protected override void SetColor(int stateNumber)
	{
		element.color = states[stateNumber];
	}
}

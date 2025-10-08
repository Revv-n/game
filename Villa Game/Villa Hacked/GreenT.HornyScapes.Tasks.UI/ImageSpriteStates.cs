using UnityEngine.UI;

namespace GreenT.HornyScapes.Tasks.UI;

public class ImageSpriteStates : SpriteStatableComponent<Image>
{
	protected override void SetSprite(int stateNumber)
	{
		element.sprite = states[stateNumber];
	}
}

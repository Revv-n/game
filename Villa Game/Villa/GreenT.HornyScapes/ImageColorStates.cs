using UnityEngine.UI;

namespace GreenT.HornyScapes;

public class ImageColorStates : ColorStatableComponent<Image>
{
	protected override void SetColor(int numberState)
	{
		element.color = states[numberState];
	}
}

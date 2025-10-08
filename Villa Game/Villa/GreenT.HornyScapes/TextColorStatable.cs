using TMPro;

namespace GreenT.HornyScapes;

public class TextColorStatable : ColorStatableComponent<TMP_Text>
{
	protected override void SetColor(int stateNumber)
	{
		element.color = states[stateNumber];
	}
}

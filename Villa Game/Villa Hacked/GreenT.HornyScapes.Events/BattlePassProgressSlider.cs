using GreenT.HornyScapes.UI;

namespace GreenT.HornyScapes.Events;

public class BattlePassProgressSlider : ProgressSlider
{
	private int target;

	public void Initialization(int target)
	{
		this.target = target;
	}

	public void SetProgress(int current)
	{
		Init(current, target, 0f);
	}
}

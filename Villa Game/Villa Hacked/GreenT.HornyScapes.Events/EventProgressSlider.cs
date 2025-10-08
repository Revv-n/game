using GreenT.HornyScapes.UI;
using TMPro;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class EventProgressSlider : ProgressSlider
{
	[SerializeField]
	private TMP_Text currentValueText;

	[SerializeField]
	private TMP_Text maxValueText;

	public void SetProgress(BaseReward source, int currencyTarget)
	{
		int target = source.Target;
		currentValueText.text = currencyTarget.ToString();
		maxValueText.text = target.ToString();
		int num = source.PrevReward?.Target ?? 0;
		int num2 = currencyTarget - num;
		int num3 = target - num;
		Init(num2, num3, 0f);
	}
}

using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.UI;

public class ProgressSlider : FloatClampedProgress
{
	[SerializeField]
	private Slider slider;

	public Slider Slider => slider;

	public override void Init(float relativeProgress)
	{
		base.Init(relativeProgress);
		slider.value = Clamp(relativeProgress);
	}
}

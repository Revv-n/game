using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.UI;

public class ProgressBar : FloatClampedProgress
{
	[SerializeField]
	private Image image;

	public override void Init(float relativeProgress)
	{
		base.Init(relativeProgress);
		image.fillAmount = Clamp(relativeProgress);
	}
}

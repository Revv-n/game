using UnityEngine;

namespace GreenT.HornyScapes.UI;

public class FloatClampedProgress : ClampedProgress<float>
{
	private float progress;

	public override float Progress => progress;

	public override void Init(float value, float max, float min = 0f)
	{
		if (max != 0f)
		{
			if (min > max)
			{
				Init(1f);
			}
			else
			{
				Init((value - min) / (max - min));
			}
		}
	}

	public override void Init(float relativeProgress)
	{
		progress = Mathf.Clamp01(relativeProgress);
	}

	public override bool IsComplete()
	{
		return progress == 1f;
	}
}

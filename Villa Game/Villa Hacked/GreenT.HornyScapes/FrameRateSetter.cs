using UnityEngine;

namespace GreenT.HornyScapes;

public class FrameRateSetter
{
	public void SetFrameRate(int frameRate = 30)
	{
		Application.targetFrameRate = frameRate;
	}
}

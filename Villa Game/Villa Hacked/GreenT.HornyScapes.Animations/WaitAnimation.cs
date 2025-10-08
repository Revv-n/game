namespace GreenT.HornyScapes.Animations;

public class WaitAnimation : Animation
{
	public override void ResetToAnimStart()
	{
		Stop();
	}
}

namespace GreenT.HornyScapes.Animations;

public class AnimationAutoStarter : AnimationStarter
{
	protected virtual void Awake()
	{
		Init();
	}

	protected virtual void OnEnable()
	{
		LaunchStarter();
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	protected virtual void OnDisable()
	{
		ResetAnimation();
	}
}

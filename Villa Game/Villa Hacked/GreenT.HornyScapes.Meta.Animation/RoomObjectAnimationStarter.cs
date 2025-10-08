using GreenT.HornyScapes.Animations;

namespace GreenT.HornyScapes.Meta.Animation;

public class RoomObjectAnimationStarter : AnimationStarter
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

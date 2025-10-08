using System.Collections.Generic;

namespace GreenT.HornyScapes.Animations;

public class RoomObjectAnimationGroup : ChainAnimationGroupOld
{
	public int Count => animations.Count;

	public void SetAnimations(List<Animation> animations)
	{
		base.animations = animations;
	}
}

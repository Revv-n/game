using System.Linq;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class AnimationGroupDelayManager : MonoBehaviour
{
	[SerializeField]
	private float delay;

	[SerializeField]
	private float delayBetweenElements = 0.5f;

	[SerializeField]
	private AnimationGroupID affectedGroupID;

	private Animation[] animations;

	public void UpdateGroupChildren()
	{
		animations = (from _animation in base.transform.GetComponentsInChildren<Animation>(includeInactive: true)
			where _animation.GroupID == affectedGroupID
			select _animation).ToArray();
		for (int i = 0; i < animations.Length; i++)
		{
			animations[i].Delay = delay + delayBetweenElements * (float)i;
		}
	}

	protected void OnTransformChildrenChanged()
	{
		UpdateGroupChildren();
	}
}

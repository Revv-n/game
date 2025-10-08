using GreenT.HornyScapes.Animations;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public abstract class TaskBaseAnimateBezier<T> : BezierAnimate where T : AnimationController
{
	[Inject]
	private T animController;

	private void Awake()
	{
		animController.Init();
	}

	protected override void AnimateTargetOnEnd(Transform target)
	{
		base.AnimateTargetOnEnd(target);
		animController.PlayAllAnimations();
	}
}

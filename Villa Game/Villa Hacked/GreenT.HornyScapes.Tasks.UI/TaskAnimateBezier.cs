using GreenT.HornyScapes.Animations;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskAnimateBezier : BezierAnimate
{
	[Inject]
	private StarAnimController starAnimController;

	private void Awake()
	{
		starAnimController.Init();
	}

	protected override void AnimateTargetOnEnd(Transform target)
	{
		base.AnimateTargetOnEnd(target);
		starAnimController.PlayAllAnimations();
	}
}

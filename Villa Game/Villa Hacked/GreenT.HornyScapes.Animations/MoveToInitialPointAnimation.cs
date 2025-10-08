using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class MoveToInitialPointAnimation : Animation
{
	public float Duration = 1f;

	[SerializeField]
	private Ease Ease = Ease.Linear;

	[SerializeField]
	private Transform child;

	private Vector3 targetPosition;

	private Vector3 startPosition;

	public override void Init()
	{
		base.Init();
		targetPosition = child.position;
	}

	public override void ResetToAnimStart()
	{
		child.position = startPosition;
	}

	public override Sequence Play()
	{
		startPosition = child.position;
		sequence = base.Play().Append(child.DOMove(targetPosition, Duration).SetEase(Ease));
		return sequence;
	}
}

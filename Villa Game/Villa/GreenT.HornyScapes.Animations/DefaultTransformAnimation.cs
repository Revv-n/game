using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class DefaultTransformAnimation : TransformAnimation
{
	public float Duration = 1f;

	public override Sequence Play()
	{
		sequence = base.Play().Append(Transform.DOLocalMove(Data.Position, Duration).SetEase(Ease)).Join(Transform.DOLocalRotate(Data.Rotation, Duration).SetEase(Ease))
			.Join(Transform.DOScale(Data.Scale, Duration).SetEase(Ease))
			.SetLoops(LoopCount, LoopType)
			.OnComplete(Complete);
		return sequence;
	}

	public override void ResetToAnimStart()
	{
		Stop();
		Transform.localPosition = base.InitialData.Position;
		base.transform.rotation = Quaternion.Euler(base.InitialData.Rotation);
		base.transform.localScale = base.InitialData.Scale;
	}
}

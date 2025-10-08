using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class NoAlphaDefaultAnimation : RectTransformAnimation
{
	[Header("EditorOnly. Назначение.")]
	[SerializeField]
	private string description;

	[SerializeField]
	private bool debug;

	public override Sequence Play()
	{
		_ = debug;
		sequence = base.Play().Append(RectTransform.DOAnchorPos(Data.Position, Duration).SetEase(Ease)).Join(RectTransform.DORotate(Data.Rotation, Duration).SetEase(Ease))
			.Join(RectTransform.DOScale(Data.Scale, Duration).SetEase(Ease))
			.SetLoops(LoopCount, LoopType);
		return sequence;
	}

	public override void ResetToAnimStart()
	{
		if (base.InitialData == null)
		{
			Init();
		}
		RectTransform.anchoredPosition = base.InitialData.Position;
		base.transform.rotation = Quaternion.Euler(base.InitialData.Rotation);
		base.transform.localScale = base.InitialData.Scale;
	}
}

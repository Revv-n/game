using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class RelativeSimpleAnimation : RectTransformAnimation
{
	public override Sequence Play()
	{
		Vector3 relativePosition = new Vector3(base.InitialData.Position.x * Data.Position.x, base.InitialData.Position.y * Data.Position.y, base.InitialData.Position.z * Data.Position.z);
		Vector3 relativeRotation = new Vector3(base.InitialData.Rotation.x * Data.Rotation.x, base.InitialData.Rotation.y * Data.Rotation.y, base.InitialData.Rotation.z * Data.Rotation.z);
		Vector3 relativeScale = new Vector3(base.InitialData.Scale.x * Data.Scale.x, base.InitialData.Scale.y * Data.Scale.y, base.InitialData.Scale.z * Data.Scale.z);
		float relativeAlpha = Mathf.Clamp01(base.InitialData.Alpha + Data.Alpha);
		sequence = base.Play();
		if (Duration == 0f)
		{
			sequence.AppendCallback(delegate
			{
				SetValues(new AnimationData(relativePosition, relativeRotation, relativeScale, relativeAlpha));
			});
		}
		else
		{
			sequence.Append(RectTransform.DOAnchorPos(relativePosition, Duration).SetEase(Ease)).Join(RectTransform.DOLocalRotate(relativeRotation, Duration).SetEase(Ease)).Join(RectTransform.DOScale(relativeScale, Duration).SetEase(Ease))
				.Join(CanvasGroup.DOFade(relativeAlpha, Duration).SetEase(Ease))
				.SetLoops(LoopCount, LoopType)
				.OnComplete(Complete);
		}
		return sequence;
	}

	private void SetValues(AnimationData data)
	{
		RectTransform.anchoredPosition = data.Position;
		RectTransform.localRotation = Quaternion.Euler(data.Rotation);
		RectTransform.localScale = data.Scale;
		CanvasGroup.alpha = data.Alpha;
	}

	public override void ResetToAnimStart()
	{
		Stop();
		if (base.InitialData != null)
		{
			SetValues(base.InitialData);
		}
	}
}

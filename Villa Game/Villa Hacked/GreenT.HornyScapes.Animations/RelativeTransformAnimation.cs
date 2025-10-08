using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class RelativeTransformAnimation : TransformAnimation
{
	public float Duration = 1f;

	public override Sequence Play()
	{
		Vector3 vector = new Vector3(base.InitialData.Position.x * Data.Position.x, base.InitialData.Position.y * Data.Position.y, base.InitialData.Position.z * Data.Position.z);
		Vector3 vector2 = new Vector3(base.InitialData.Rotation.x * Data.Rotation.x, base.InitialData.Rotation.y * Data.Rotation.y, base.InitialData.Rotation.z * Data.Rotation.z);
		Vector3 vector3 = new Vector3(base.InitialData.Scale.x * Data.Scale.x, base.InitialData.Scale.y * Data.Scale.y, base.InitialData.Scale.z * Data.Scale.z);
		float num = Mathf.Clamp01(base.InitialData.Alpha + Data.Alpha);
		sequence = base.Play();
		if (Duration == 0f)
		{
			SetValues(new AnimationData(vector, vector2, vector3, num));
		}
		else
		{
			sequence = sequence.Append(Transform.DOLocalMove(vector, Duration).SetEase(Ease)).Join(Transform.DOLocalRotate(vector2, Duration).SetEase(Ease)).Join(Transform.DOScale(vector3, Duration).SetEase(Ease));
			if ((bool)Renderer)
			{
				sequence = sequence.Join(Renderer.material.DOFade(num, Duration).SetEase(Ease));
			}
			sequence = sequence.SetLoops(LoopCount, LoopType).OnComplete(Complete);
		}
		return sequence;
	}

	private void SetValues(AnimationData data)
	{
		Transform.localPosition = data.Position;
		base.transform.localRotation = Quaternion.Euler(data.Rotation);
		base.transform.localScale = data.Scale;
		if ((bool)Renderer)
		{
			Color color = Renderer.material.color;
			color.a = data.Alpha;
			Renderer.material.color = color;
		}
	}

	public override void ResetToAnimStart()
	{
		Stop();
		SetValues(base.InitialData);
	}
}

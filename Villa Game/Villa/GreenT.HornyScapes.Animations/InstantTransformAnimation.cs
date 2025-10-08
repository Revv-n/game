using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class InstantTransformAnimation : TransformAnimation
{
	public override Sequence Play()
	{
		base.transform.localPosition = Data.Position;
		base.transform.localRotation = Quaternion.Euler(Data.Rotation);
		base.transform.localScale = Data.Scale;
		if (Renderer != null)
		{
			Color color = Renderer.material.color;
			color.a = Data.Alpha;
			Renderer.material.color = color;
		}
		return base.Play();
	}

	public override void ResetToAnimStart()
	{
		Stop();
		Transform.localPosition = base.InitialData.Position;
		base.transform.localRotation = Quaternion.Euler(base.InitialData.Rotation);
		base.transform.localScale = base.InitialData.Scale;
		if (Renderer != null)
		{
			Color color = Renderer.material.color;
			color.a = base.InitialData.Alpha;
			Renderer.material.color = color;
		}
	}
}

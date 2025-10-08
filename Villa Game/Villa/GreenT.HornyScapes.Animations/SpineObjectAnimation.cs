using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class SpineObjectAnimation : Animation
{
	public Renderer SpineRenderer;

	public Renderer GlowRenderer;

	public Ease Ease;

	public float Duration = 1f;

	public float SpineTarget = 1f;

	public float GlowTarget = 1f;

	public override Sequence Play()
	{
		GlowRenderer.enabled = true;
		if (!Application.isPlaying)
		{
			return base.Play();
		}
		Sequence sequence = DOTween.Sequence().Append(GlowRenderer.material.DOFade(GlowTarget, Duration).SetEase(Ease));
		foreach (Material item in SpineRenderer.sharedMaterials.Where((Material _mat) => _mat.HasProperty("_Color")))
		{
			sequence.Join(item.DOFade(SpineTarget, Duration)).SetEase(Ease);
		}
		return base.Play().Append(sequence);
	}

	protected override void Complete()
	{
		base.Complete();
	}

	public override void ResetToAnimStart()
	{
	}
}

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Animations;

public class ImageDefaultSimpleAnimation : DefaultSimpleAnimation
{
	[SerializeField]
	private Image target;

	[SerializeField]
	private Color endColor;

	protected override void OnValidate()
	{
		base.OnValidate();
		if (!target && TryGetComponent<Image>(out var component))
		{
			target = component;
		}
	}

	public override Sequence Play()
	{
		sequence = base.Play().Join(target.DOColor(endColor, Duration));
		return sequence;
	}

	public override void Stop()
	{
		sequence?.Kill();
		base.Stop();
	}
}

using System;
using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

[Serializable]
public class ColorGraphicAnimation : GraphicAnimation
{
	public float Duration = 1f;

	[SerializeField]
	private Color targetColor = new Color(1f, 1f, 1f, 1f);

	private Color initColor;

	private void Awake()
	{
		initColor = graphic.color;
	}

	public override void Init()
	{
		base.Init();
		graphic.color = initColor;
	}

	public override Sequence Play()
	{
		Tween t = graphic.DOColor(targetColor, Duration);
		sequence = base.Play().Append(t);
		return sequence;
	}

	public override void ResetToAnimStart()
	{
		Init();
	}
}

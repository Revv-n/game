using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

[RequireComponent(typeof(RectTransform))]
public abstract class RectTransformAnimation : Animation
{
	public float Duration = 1f;

	public bool Inverse;

	public CanvasGroup CanvasGroup;

	public RectTransform RectTransform;

	public Ease Ease = Ease.Linear;

	public int LoopCount;

	public LoopType LoopType = LoopType.Yoyo;

	public AnimationData Data;

	public AnimationData InitialData { get; private set; }

	protected virtual void OnValidate()
	{
		if (CanvasGroup == null && TryGetComponent<CanvasGroup>(out var component))
		{
			CanvasGroup = component;
		}
		if (RectTransform == null && TryGetComponent<RectTransform>(out var component2))
		{
			RectTransform = component2;
		}
		if (!CanvasGroup || !RectTransform)
		{
			if (!CanvasGroup)
			{
				Debug.LogError("CanvasGroup weren't set " + base.gameObject.name, this);
			}
			if (!RectTransform)
			{
				Debug.LogError("RectTransform weren't set", this);
			}
		}
		else if (Data == null)
		{
			Data = new AnimationData(RectTransform.anchoredPosition, RectTransform.localRotation.eulerAngles, RectTransform.localScale, CanvasGroup.alpha);
		}
	}

	public override void Init()
	{
		AnimationData animationData = new AnimationData(RectTransform.anchoredPosition, RectTransform.localRotation.eulerAngles, RectTransform.localScale, CanvasGroup.alpha);
		InitialData = (Inverse ? Data : animationData);
		Data = (Inverse ? animationData : Data);
		base.Init();
	}

	public void SetId(int id)
	{
		AnimationId = id;
	}
}

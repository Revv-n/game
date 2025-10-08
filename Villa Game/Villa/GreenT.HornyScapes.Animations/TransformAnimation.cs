using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public abstract class TransformAnimation : Animation
{
	public bool Inverse;

	public Transform Transform;

	public Renderer Renderer;

	public Ease Ease = Ease.Linear;

	public int LoopCount;

	public LoopType LoopType = LoopType.Yoyo;

	public AnimationData Data;

	public AnimationData InitialData { get; private set; }

	protected virtual void OnValidate()
	{
		if (Transform == null && TryGetComponent<Transform>(out var component))
		{
			Transform = component;
		}
		if (Renderer == null && TryGetComponent<Renderer>(out var component2))
		{
			Renderer = component2;
		}
		float alpha = 1f;
		if (Renderer != null)
		{
			alpha = Renderer.sharedMaterial.color.a;
		}
		if (Data == null)
		{
			Data = new AnimationData(Transform.localPosition, Transform.localRotation.eulerAngles, Transform.localScale, alpha);
		}
	}

	public override void Init()
	{
		base.Init();
		float alpha = 1f;
		if (Renderer != null)
		{
			alpha = Renderer.sharedMaterial.color.a;
		}
		AnimationData animationData = new AnimationData(Transform.localPosition, Transform.localRotation.eulerAngles, Transform.localScale, alpha);
		InitialData = (Inverse ? Data : animationData);
		Data = (Inverse ? animationData : Data);
	}

	public void SetId(int id)
	{
		AnimationId = id;
	}

	[ContextMenu("Update Data")]
	protected virtual void UpdateData()
	{
		HardClear();
		OnValidate();
		void HardClear()
		{
			Transform = null;
			Data = null;
			Renderer = null;
		}
	}
}

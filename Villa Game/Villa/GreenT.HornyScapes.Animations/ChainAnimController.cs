using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class ChainAnimController : AnimationController
{
	[Header("EditorOnly. Назначение.")]
	[SerializeField]
	private string description;

	[SerializeField]
	protected Animation[] animations;

	private Sequence animationSequence;

	public override void Init()
	{
		base.Init();
		Animation[] array = animations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public override void PlayAllAnimations()
	{
		StopAll();
		animations[0].ResetToAnimStart();
		animationSequence = DOTween.Sequence();
		for (int i = 0; i != animations.Length; i++)
		{
			animationSequence = animationSequence.Append(animations[i].Play());
		}
		animationSequence.OnComplete(delegate
		{
			onPlayEnd.OnNext(this);
		});
	}

	private void StopAll()
	{
		if (animationSequence.IsActive())
		{
			animationSequence.Kill();
		}
	}

	public override void ResetAnimation()
	{
		StopAll();
		Animation animation = animations.FirstOrDefault();
		if ((bool)animation)
		{
			animation.ResetToAnimStart();
		}
	}

	private void OnDestroy()
	{
		StopAll();
	}

	protected override void OnValidate()
	{
		base.OnValidate();
		if (animations == null)
		{
			Animation[] components = GetComponents<RectTransformAnimation>();
			animations = components;
		}
		if (animations == null || !animations.FirstOrDefault())
		{
			Debug.Log("You have empty Animation controller on " + base.gameObject.name, this);
		}
	}

	public override bool IsPlaying()
	{
		return animationSequence.IsActive();
	}
}

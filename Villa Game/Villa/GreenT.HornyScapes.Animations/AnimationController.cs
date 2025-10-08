using System;
using UniRx;

namespace GreenT.HornyScapes.Animations;

public abstract class AnimationController : Animation
{
	protected Subject<AnimationController> onPlayEnd = new Subject<AnimationController>();

	public IObservable<AnimationController> OnPlayEnd => onPlayEnd.AsObservable();

	public abstract void PlayAllAnimations();

	public abstract void ResetAnimation();

	public abstract bool IsPlaying();

	protected static bool TryPlayAnimation(Animation animation)
	{
		if ((bool)animation)
		{
			animation.ResetToAnimStart();
			animation.Play();
		}
		return animation;
	}

	protected virtual void OnValidate()
	{
		RectTransformAnimation[] components = GetComponents<RectTransformAnimation>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].SetId(i);
		}
	}
}

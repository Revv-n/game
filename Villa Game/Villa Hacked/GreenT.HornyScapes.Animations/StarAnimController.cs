using System;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public class StarAnimController : AnimationController
{
	[SerializeField]
	private Animation[] starAnimations;

	[SerializeField]
	private ParticleSystem[] particles;

	public override void Init()
	{
		Animation[] array = starAnimations;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
		for (int j = 0; j < starAnimations.Length - 1; j++)
		{
			Animation nextAnim = starAnimations[j + 1];
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Animation>(Observable.TakeUntilDisable<Animation>(starAnimations[j].OnAnimationEnd, (Component)this), (Action<Animation>)delegate
			{
				nextAnim.Play();
			}), (Component)this);
		}
	}

	public override void ResetToAnimStart()
	{
		ResetAnimation();
	}

	public override bool IsPlaying()
	{
		throw new NotImplementedException();
	}

	public override void PlayAllAnimations()
	{
		starAnimations[0].ResetToAnimStart();
		starAnimations[0].Play();
		ParticleSystem[] array = particles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	public override void ResetAnimation()
	{
		starAnimations[0].ResetToAnimStart();
	}
}

using System;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Tasks.UI;

public class AnimateTaskBtnNotify : MonoBehaviour
{
	[SerializeField]
	private TaskBookNotify taskBookNotify;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation startAnimation;

	[SerializeField]
	private GreenT.HornyScapes.Animations.Animation endlessAnimation;

	private CompositeDisposable animationStream = new CompositeDisposable();

	private Sequence startAnimationSeq;

	private void Awake()
	{
		startAnimation.Init();
		taskBookNotify.OnUpdate.Subscribe(UpdateAnimation).AddTo(animationStream);
		UpdateAnimation(taskBookNotify.IsActive);
	}

	private void UpdateAnimation(bool state)
	{
		if (state)
		{
			startAnimationSeq = startAnimation.Play();
			Sequence sequence = startAnimationSeq;
			sequence.onComplete = (TweenCallback)Delegate.Combine(sequence.onComplete, new TweenCallback(StartEndlessAnimation));
		}
		else
		{
			startAnimation.Stop();
			endlessAnimation.Stop();
		}
	}

	private void StartEndlessAnimation()
	{
		endlessAnimation.Play();
	}

	private void OnDestroy()
	{
		animationStream.Dispose();
	}
}

using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Collections.Promote.UI.Animation;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Collections.Promote.UI;

public class AnimatedPromoteButton : PromoteButton
{
	[SerializeField]
	private AnimationController promoteOnEndController;

	[SerializeField]
	private AnimationStarter starter;

	[SerializeField]
	private AnimatedPromoteCardView cardClap;

	private CompositeDisposable animationStream = new CompositeDisposable();

	protected void Start()
	{
		starter.Init();
	}

	protected override void Promote()
	{
		promoteButton.enabled = false;
		starter.PlayAnimation();
		cardClap.AnimateCard();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationDependedStarter>(Observable.DoOnCompleted<AnimationDependedStarter>(Observable.DoOnCancel<AnimationDependedStarter>(Observable.First<AnimationDependedStarter>(starter.OnEnd), (Action)starter.ResetAnimation), (Action)starter.ResetAnimation)), (ICollection<IDisposable>)animationStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<AnimationController>(Observable.DoOnCancel<AnimationController>(Observable.First<AnimationController>(promoteOnEndController.OnPlayEnd), (Action)delegate
		{
			base.Promote();
			promoteButton.enabled = true;
		}), (Action<AnimationController>)delegate
		{
			base.Promote();
			promoteButton.enabled = true;
		}), (ICollection<IDisposable>)animationStream);
	}
}

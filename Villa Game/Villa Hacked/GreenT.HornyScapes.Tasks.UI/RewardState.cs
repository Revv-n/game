using System;
using DG.Tweening;
using GreenT.HornyScapes.Animations;
using StripClub.Model;
using StripClub.Model.Quest;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public class RewardState : TaskViewState
{
	public TaskView TaskView;

	public Image Star;

	public RectTransformAnimation HideAnimation;

	public LayoutElement LayoutElement;

	private Sequence applyAnimationSequence;

	private TaskStarFly taskStarFly;

	private IDisposable setComplete;

	[Inject]
	private void Constructor(TaskStarFly taskStarFly)
	{
		this.taskStarFly = taskStarFly;
	}

	protected void Awake()
	{
		HideAnimation.Init();
	}

	public override void Enter()
	{
		base.Enter();
		GiveReward();
		HideView();
	}

	private void GiveReward()
	{
		if (!(source.Reward is LootboxLinkedContent))
		{
			CurrencyLinkedContent next = source.Reward.GetNext<CurrencyLinkedContent>(checkThis: true);
			if (next == null)
			{
				return;
			}
			applyAnimationSequence = taskStarFly.LaunchStar(Star.transform, next.Currency, next.Quantity);
			if (applyAnimationSequence == null)
			{
				OnCurrencyAnimationFinished();
				return;
			}
			Sequence sequence = applyAnimationSequence;
			sequence.onKill = (TweenCallback)Delegate.Combine(sequence.onKill, (TweenCallback)delegate
			{
				OnCurrencyAnimationFinished();
			});
		}
		else
		{
			OnCurrencyAnimationFinished();
		}
	}

	private void HideView()
	{
		HideAnimation.Play();
		setComplete?.Dispose();
		setComplete = DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Animation>(Observable.FirstOrDefault<Animation>(HideAnimation.OnAnimationEnd), (Action<Animation>)delegate
		{
			try
			{
				LayoutElement.ignoreLayout = true;
			}
			catch (Exception innerException)
			{
				throw innerException.SendException(GetType().Name + " error on complete animation");
			}
		}), (Component)this);
	}

	private void OnCurrencyAnimationFinished()
	{
		source.SelectState(StateType.Rewarded);
		source.Reward.AddCurrentToPlayer();
		TaskView.IsInPool = true;
		source.ForceUpdate();
	}
}

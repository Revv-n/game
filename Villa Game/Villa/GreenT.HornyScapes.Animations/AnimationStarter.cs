using System;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

public abstract class AnimationStarter : Animation
{
	[Header("Описание анимации. EditorOnly")]
	[SerializeField]
	protected string description;

	[SerializeField]
	protected AnimationController controller;

	public Animation startPoint;

	protected Subject<AnimationDependedStarter> onEnd = new Subject<AnimationDependedStarter>();

	private IDisposable launchStream;

	public IObservable<AnimationDependedStarter> OnEnd => onEnd.AsObservable();

	protected virtual void OnValidate()
	{
		if (controller == null && TryGetComponent<AnimationController>(out var component))
		{
			controller = component;
		}
		else if (controller == null)
		{
			Debug.LogError("Starter can't have empty a controller", this);
		}
	}

	public override void Init()
	{
		base.Init();
		controller.Init();
	}

	public virtual void PlayAnimation()
	{
		controller.PlayAllAnimations();
	}

	public virtual void ResetAnimation()
	{
		controller.ResetAnimation();
	}

	public void LaunchStarter()
	{
		launchStream?.Dispose();
		PlayAnimation();
		launchStream = OnEnd.First().DoOnCancel(delegate
		{
			ResetAnimation();
		}).Subscribe(delegate
		{
			ResetAnimation();
		}, delegate(Exception ex)
		{
			ex.LogException();
		});
	}

	public void BreakLaunch()
	{
		launchStream?.Dispose();
	}

	public virtual void SetController(AnimationController newController)
	{
		controller = newController;
	}

	private void OnDestroy()
	{
		onEnd?.Dispose();
	}
}

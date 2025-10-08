using System;
using GreenT.HornyScapes.Animations;
using UniRx;
using UnityEngine;

namespace GreenT.HornyScapes.UI.Promote.Animation;

public class AnimatedPictureSwitcher : PictureSwitcher
{
	[SerializeField]
	private AnimationDependedStarter starter;

	[SerializeField]
	private AnimationStarterSetter setter;

	[SerializeField]
	private AnimationController onEndCardClapController;

	private IDisposable unlockToggleStream;

	private IDisposable beforeUnlockStream;

	protected override void UnlockToggle(ToggleWithLocker toggle, bool isLock)
	{
		unlockToggleStream?.Dispose();
		unlockToggleStream = ObservableExtensions.Subscribe<AnimationDependedStarter>(Observable.Catch<AnimationDependedStarter, Exception>(Observable.DoOnCompleted<AnimationDependedStarter>(Observable.DoOnCancel<AnimationDependedStarter>(Observable.DoOnSubscribe<AnimationDependedStarter>(Observable.First<AnimationDependedStarter>(starter.OnEnd), (Action)delegate
		{
			setter.SetController(characterSettings.AvatarNumber);
			starter.Init();
			starter.PlayAnimation();
		}), (Action)delegate
		{
			base.UnlockToggle(toggle, isLock);
			starter.ResetAnimation();
		}), (Action)delegate
		{
			base.UnlockToggle(toggle, isLock);
		}), (Func<Exception, IObservable<AnimationDependedStarter>>)delegate(Exception innerEx)
		{
			throw innerEx.SendException("Can't animate UnlockToggle in Promote");
		}));
	}

	protected override void BeforeUnlockNewToggle(ToggleWithLocker toggle, int toggleIndex, bool newState)
	{
		beforeUnlockStream?.Dispose();
		beforeUnlockStream = ObservableExtensions.Subscribe<AnimationController>(Observable.DoOnCancel<AnimationController>(Observable.First<AnimationController>(onEndCardClapController.OnPlayEnd), (Action)delegate
		{
			base.BeforeUnlockNewToggle(toggle, toggleIndex, newState);
		}), (Action<AnimationController>)delegate
		{
			base.BeforeUnlockNewToggle(toggle, toggleIndex, newState);
		}, (Action<Exception>)delegate(Exception ex)
		{
			ex.LogException();
		});
	}
}

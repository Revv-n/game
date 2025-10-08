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
		unlockToggleStream = starter.OnEnd.First().DoOnSubscribe(delegate
		{
			setter.SetController(characterSettings.AvatarNumber);
			starter.Init();
			starter.PlayAnimation();
		}).DoOnCancel(delegate
		{
			base.UnlockToggle(toggle, isLock);
			starter.ResetAnimation();
		})
			.DoOnCompleted(delegate
			{
				base.UnlockToggle(toggle, isLock);
			})
			.Catch(delegate(Exception innerEx)
			{
				throw innerEx.SendException("Can't animate UnlockToggle in Promote");
			})
			.Subscribe();
	}

	protected override void BeforeUnlockNewToggle(ToggleWithLocker toggle, int toggleIndex, bool newState)
	{
		beforeUnlockStream?.Dispose();
		beforeUnlockStream = onEndCardClapController.OnPlayEnd.First().DoOnCancel(delegate
		{
			base.BeforeUnlockNewToggle(toggle, toggleIndex, newState);
		}).Subscribe(delegate
		{
			base.BeforeUnlockNewToggle(toggle, toggleIndex, newState);
		}, delegate(Exception ex)
		{
			ex.LogException();
		});
	}
}

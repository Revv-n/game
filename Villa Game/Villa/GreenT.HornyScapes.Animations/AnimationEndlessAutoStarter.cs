using System;
using UniRx;

namespace GreenT.HornyScapes.Animations;

public class AnimationEndlessAutoStarter : AnimationAutoStarter
{
	private IDisposable endlessStream;

	protected override void OnEnable()
	{
		base.OnEnable();
		endlessStream?.Dispose();
		controller.OnPlayEnd.TakeUntilDisable(this).DoOnCancel(ResetAnimation).Subscribe(delegate
		{
			base.OnEnable();
		});
	}

	protected override void OnDisable()
	{
		endlessStream?.Dispose();
		base.OnDisable();
	}
}

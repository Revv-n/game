using System;
using UniRx;

namespace GreenT.HornyScapes.Animations;

[Serializable]
public class AnimationSetOpenClose
{
	public Animation OpenStarter;

	public Animation CloseStarter;

	public IObservable<AnimationSetOpenClose> Open()
	{
		OpenStarter.ResetToAnimStart();
		OpenStarter.Play();
		return from _ in OpenStarter.OnAnimationEnd.First()
			select this;
	}

	public IObservable<AnimationSetOpenClose> Close()
	{
		CloseStarter.ResetToAnimStart();
		CloseStarter.Play();
		return from _ in CloseStarter.OnAnimationEnd.First()
			select this;
	}
}

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
		return Observable.Select<Animation, AnimationSetOpenClose>(Observable.First<Animation>(OpenStarter.OnAnimationEnd), (Func<Animation, AnimationSetOpenClose>)((Animation _) => this));
	}

	public IObservable<AnimationSetOpenClose> Close()
	{
		CloseStarter.ResetToAnimStart();
		CloseStarter.Play();
		return Observable.Select<Animation, AnimationSetOpenClose>(Observable.First<Animation>(CloseStarter.OnAnimationEnd), (Func<Animation, AnimationSetOpenClose>)((Animation _) => this));
	}
}

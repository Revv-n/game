using System;
using DG.Tweening;

namespace GreenT.HornyScapes.Animations;

public interface IAnimation
{
	IObservable<Animation> OnAnimationEnd { get; }

	Sequence Play();

	void Stop();

	void Init();
}

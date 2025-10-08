using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GreenT.HornyScapes.Animations;

[Serializable]
public class AnimationSetOpenCloseController
{
	private Subject<AnimationSetOpenCloseController> onStartersEnd = new Subject<AnimationSetOpenCloseController>();

	public List<AnimationSetOpenClose> starters;

	public IObservable<AnimationSetOpenCloseController> OnStartersEnd => Observable.AsObservable<AnimationSetOpenCloseController>((IObservable<AnimationSetOpenCloseController>)onStartersEnd);

	public void InitAll()
	{
		foreach (AnimationSetOpenClose starter in starters)
		{
			starter.OpenStarter.Init();
			starter.CloseStarter.Init();
		}
	}

	public void InitOpeners()
	{
		foreach (AnimationSetOpenClose starter in starters)
		{
			starter.OpenStarter.Init();
		}
	}

	public void InitClosers()
	{
		foreach (AnimationSetOpenClose starter in starters)
		{
			starter.CloseStarter.Init();
		}
	}

	public IObservable<AnimationSetOpenCloseController> Open()
	{
		return Observable.Select<AnimationSetOpenClose, AnimationSetOpenCloseController>(Observable.DefaultIfEmpty<AnimationSetOpenClose>(Observable.Merge<AnimationSetOpenClose>(starters.Select((AnimationSetOpenClose _starter) => _starter.Open()))), (Func<AnimationSetOpenClose, AnimationSetOpenCloseController>)((AnimationSetOpenClose _) => this));
	}

	public IObservable<AnimationSetOpenCloseController> Close()
	{
		return Observable.Select<AnimationSetOpenClose, AnimationSetOpenCloseController>(Observable.DefaultIfEmpty<AnimationSetOpenClose>(Observable.Merge<AnimationSetOpenClose>(starters.Select((AnimationSetOpenClose _starter) => _starter.Close()))), (Func<AnimationSetOpenClose, AnimationSetOpenCloseController>)((AnimationSetOpenClose _) => this));
	}
}

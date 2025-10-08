using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Tutorial;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class TutorialGroupSaveEvent : SaveEvent
{
	private TutorialGroupManager tutorManager;

	private GameStarter gameStarter;

	[Inject]
	public void Init(TutorialGroupManager tutorManager, GameStarter gameStarter)
	{
		this.tutorManager = tutorManager;
		this.gameStarter = gameStarter;
	}

	public override void Track()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>(Observable.SelectMany<bool, bool>(Observable.Where<bool>(Observable.Skip<bool>((IObservable<bool>)gameStarter.IsGameReadyToStart, 1), (Func<bool, bool>)((bool x) => x)), Observable.SelectMany<TutorialGroupSteps, bool>(tutorManager.GetUncompletedGroupObservable(), (Func<TutorialGroupSteps, IObservable<bool>>)((TutorialGroupSteps _group) => (IObservable<bool>)_group.IsCompleted))), (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}

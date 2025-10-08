using System;
using System.Linq;
using GreenT.HornyScapes.Constants;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Tutorial;

public class TutorialFinisherResolver : IDisposable
{
	private readonly GameStarter gameStarter;

	private readonly TutorialSystem tutorialSystem;

	private readonly TutorialGroupManager tutorManager;

	private readonly IConstants<ILocker> lockerConstant;

	private readonly string autoEnd;

	private IDisposable onGameStartedStream;

	public TutorialFinisherResolver(GameStarter gameStarter, TutorialSystem tutorialSystem, TutorialGroupManager tutorManager, IConstants<ILocker> lockerConstant, string autoEnd)
	{
		this.gameStarter = gameStarter;
		this.tutorialSystem = tutorialSystem;
		this.tutorManager = tutorManager;
		this.lockerConstant = lockerConstant;
		this.autoEnd = autoEnd;
	}

	public void Initialize()
	{
		onGameStartedStream?.Dispose();
		onGameStartedStream = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			TryToStartTutorial();
		});
	}

	private void TryToStartTutorial()
	{
		if (lockerConstant[autoEnd].IsOpen.Value)
		{
			SetAsCompleted();
		}
		else
		{
			tutorialSystem.StartListen();
		}
	}

	private void SetAsCompleted()
	{
		TutorialGroupSteps[] array = tutorManager.Collection.Where((TutorialGroupSteps _group) => !_group.IsCompleted.Value).ToArray();
		TutorialGroupSteps[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].IsCompleted.Value = true;
		}
		array.Any();
	}

	public void Dispose()
	{
		onGameStartedStream?.Dispose();
	}
}

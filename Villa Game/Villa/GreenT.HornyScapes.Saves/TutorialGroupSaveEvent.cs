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
		(from x in (from x in gameStarter.IsGameReadyToStart.Skip(1)
				where x
				select x).SelectMany(tutorManager.GetUncompletedGroupObservable().SelectMany((TutorialGroupSteps _group) => _group.IsCompleted))
			where x
			select x).Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}

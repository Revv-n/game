using System.Linq;
using GreenT.HornyScapes.MiniEvents;
using GreenT.HornyScapes.Tasks;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class MinieventTaskCompleteSaveEvent : SaveEvent
{
	private MiniEventTaskManager _miniEventTaskManager;

	[Inject]
	public void Init(MiniEventTaskManager miniEventTaskManager)
	{
		_miniEventTaskManager = miniEventTaskManager;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		(from task in _miniEventTaskManager.Collection.Where((MiniEventTask t) => !t.IsRewarded).ToObservable().SelectMany((MiniEventTask task) => task.OnUpdate)
				.Merge()
			where task.IsRewarded
			select task).Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}

using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Saves;

public class TaskCompleteSaveEvent : SaveEvent
{
	private TaskStateProvider taskStateProvider;

	[Inject]
	public void Init(TaskStateProvider taskStateProvider)
	{
		this.taskStateProvider = taskStateProvider;
	}

	public override void Track()
	{
		Initialize();
	}

	private void Initialize()
	{
		taskStateProvider.OnRewardedAll().Subscribe(delegate
		{
			Save();
		}).AddTo(saveStreams);
	}
}

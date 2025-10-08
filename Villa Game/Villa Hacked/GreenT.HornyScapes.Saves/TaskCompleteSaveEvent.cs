using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Tasks;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(taskStateProvider.OnRewardedAll(), (Action<Task>)delegate
		{
			Save();
		}), (ICollection<IDisposable>)saveStreams);
	}
}

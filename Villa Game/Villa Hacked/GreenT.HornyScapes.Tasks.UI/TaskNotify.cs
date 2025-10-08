using System;
using System.Collections.Generic;
using UniRx;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskNotify : ControllerNotify<TaskController>
{
	protected override void ListenEvents()
	{
		base.ListenEvents();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Task>(controller.OnUpdate, (Action<Task>)delegate
		{
			ActivateNotify();
		}), (ICollection<IDisposable>)notifyStream);
	}

	public void ActivateNotify()
	{
		bool state = controller.HasAnyTaskReadyCompleted();
		SetState(state);
	}
}

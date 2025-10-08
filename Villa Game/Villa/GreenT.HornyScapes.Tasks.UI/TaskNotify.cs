using UniRx;

namespace GreenT.HornyScapes.Tasks.UI;

public class TaskNotify : ControllerNotify<TaskController>
{
	protected override void ListenEvents()
	{
		base.ListenEvents();
		controller.OnUpdate.Subscribe(delegate
		{
			ActivateNotify();
		}).AddTo(notifyStream);
	}

	public void ActivateNotify()
	{
		bool state = controller.HasAnyTaskReadyCompleted();
		SetState(state);
	}
}

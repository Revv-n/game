using GreenT.HornyScapes.Tasks.UI;
using UniRx;

namespace GreenT.HornyScapes.Messenger;

public class ResponseNotify : ControllerNotify<MessengerNotifyController>
{
	protected override void Awake()
	{
		base.Awake();
		ActivateNotify();
	}

	private void ActivateNotify()
	{
		bool hasPlayerResponse = controller.HasPlayerResponse;
		SetState(hasPlayerResponse);
	}

	protected override void ListenEvents()
	{
		base.ListenEvents();
		controller.OnHasPlayerResponse.Subscribe(base.SetState).AddTo(notifyStream);
	}
}

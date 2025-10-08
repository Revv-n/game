using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(controller.OnHasPlayerResponse, (Action<bool>)base.SetState), (ICollection<IDisposable>)notifyStream);
	}
}

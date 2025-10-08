using Zenject;

namespace GreenT.HornyScapes.Tasks.UI;

public abstract class ControllerNotify<T> : BaseNotify
{
	protected T controller;

	[Inject]
	protected virtual void InnerInit(T controller)
	{
		this.controller = controller;
	}

	protected virtual void Awake()
	{
		SetState(activate: false);
		ListenEvents();
	}
}

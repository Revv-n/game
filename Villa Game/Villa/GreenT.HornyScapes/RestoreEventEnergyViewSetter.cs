using Zenject;

namespace GreenT.HornyScapes;

public class RestoreEventEnergyViewSetter : IInitializable
{
	private RestoreEventEnergyViewController _restoreEventEnergyViewController;

	private RestoreEventEnergyView _restoreEventEnergyView;

	[Inject]
	public void Init(RestoreEventEnergyViewController restoreEventEnergyViewController, RestoreEventEnergyView restoreEventEnergyView)
	{
		_restoreEventEnergyView = restoreEventEnergyView;
		_restoreEventEnergyViewController = restoreEventEnergyViewController;
	}

	public void Initialize()
	{
		_restoreEventEnergyViewController.SetView(_restoreEventEnergyView);
	}
}

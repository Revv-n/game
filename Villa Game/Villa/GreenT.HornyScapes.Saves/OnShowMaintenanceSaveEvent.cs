using GreenT.HornyScapes.Maintenance;
using UniRx;

namespace GreenT.HornyScapes.Saves;

public class OnShowMaintenanceSaveEvent : SaveEvent
{
	private MaintenanceListener _listener;

	private GameStarter gameStarter;

	public override void Track()
	{
		if (_listener != null)
		{
			gameStarter.IsGameActive.First((bool _isActive) => _isActive).ContinueWith(_listener.NeedResetClient).Subscribe(delegate
			{
				Save();
			})
				.AddTo(saveStreams);
		}
	}
}

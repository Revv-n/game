using System;
using System.Collections.Generic;
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
			DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<MaintenanceInfo>(Observable.ContinueWith<bool, MaintenanceInfo>(Observable.First<bool>((IObservable<bool>)gameStarter.IsGameActive, (Func<bool, bool>)((bool _isActive) => _isActive)), _listener.NeedResetClient), (Action<MaintenanceInfo>)delegate
			{
				Save();
			}), (ICollection<IDisposable>)saveStreams);
		}
	}
}

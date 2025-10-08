using System;
using GreenT.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Maintenance.UI;

public class DisplayMaintenancePopup : IInitializable, IDisposable
{
	private readonly MaintenanceListener _maintenanceListener;

	private readonly IWindowsManager _windowsManager;

	private readonly MaintenanceWindowOpener _maintenanceWindowOpener;

	private readonly GameStarter _gameStarter;

	public ReactiveProperty<MaintenanceInfo> Reason = new ReactiveProperty<MaintenanceInfo>();

	private bool _isMaintenanceSet;

	private IDisposable _showStream;

	private DisplayMaintenancePopup(MaintenanceListener maintenanceListener, IWindowsManager windowsManager, MaintenanceWindowOpener maintenanceWindowOpener, GameStarter gameStarter)
	{
		_maintenanceListener = maintenanceListener;
		_windowsManager = windowsManager;
		_maintenanceWindowOpener = maintenanceWindowOpener;
		_gameStarter = gameStarter;
	}

	public void Initialize()
	{
		_showStream?.Dispose();
		_showStream = _maintenanceListener.NeedResetClient.Subscribe(SetMT);
		TrackNewOpenedWindow();
	}

	private void SetMT(MaintenanceInfo reason)
	{
		Reason.Value = reason;
		Show();
		_isMaintenanceSet = true;
	}

	private void TrackNewOpenedWindow()
	{
		_windowsManager.OnOpenWindow += UpMaintenance;
	}

	private void UpMaintenance(IWindow window)
	{
		if (_isMaintenanceSet)
		{
			Show();
		}
	}

	private void Show()
	{
		_maintenanceWindowOpener.OpenAdditive();
	}

	public void Dispose()
	{
		if (_windowsManager != null)
		{
			_windowsManager.OnOpenWindow -= UpMaintenance;
		}
		_showStream?.Dispose();
	}
}

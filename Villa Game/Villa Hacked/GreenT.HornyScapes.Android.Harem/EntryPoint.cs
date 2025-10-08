using System;
using GreenT.HornyScapes.Maintenance;
using GreenT.HornyScapes.Saves;
using GreenT.Net.User;

namespace GreenT.HornyScapes.Android.Harem;

public class EntryPoint : BaseEntryPoint
{
	private readonly MaintenanceListener _maintenanceListener;

	private readonly FrameRateSetter _frameRateSetter;

	public EntryPoint(GameController gameController, SaveController saveController, RestoreSessionProcessor restoreSessionProcessor, MaintenanceListener maintenanceListener, FrameRateSetter frameRateSetter)
		: base(gameController, saveController, restoreSessionProcessor)
	{
		_maintenanceListener = maintenanceListener;
		_frameRateSetter = frameRateSetter;
	}

	public override void Initialize()
	{
		_frameRateSetter.SetFrameRate();
		try
		{
			base.Initialize();
			_maintenanceListener.Track();
		}
		catch (Exception exception)
		{
			throw exception.LogException();
		}
	}
}

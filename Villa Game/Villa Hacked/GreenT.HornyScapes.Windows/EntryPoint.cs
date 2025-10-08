using System;
using GreenT.HornyScapes.Exceptions;
using GreenT.HornyScapes.Maintenance;
using GreenT.HornyScapes.Saves;
using GreenT.Net.User;
using GreenT.Steam;

namespace GreenT.HornyScapes.Windows;

public class EntryPoint : BaseEntryPoint
{
	private readonly MaintenanceListener _maintenanceListener;

	private readonly ISteamBridge _steamBridge;

	private readonly IExceptionHandler _exceptionHandler;

	private readonly FrameRateSetter _frameRateSetter;

	public EntryPoint(MaintenanceListener maintenanceListener, ISteamBridge steamBridge, IExceptionHandler exceptionHandler, GameController gameController, SaveController saveController, RestoreSessionProcessor restoreSessionProcessor, FrameRateSetter frameRateSetter)
		: base(gameController, saveController, restoreSessionProcessor)
	{
		_maintenanceListener = maintenanceListener;
		_steamBridge = steamBridge;
		_exceptionHandler = exceptionHandler;
		_frameRateSetter = frameRateSetter;
	}

	public override void Initialize()
	{
		_frameRateSetter.SetFrameRate(120);
		InitAuth();
	}

	private void InitAuth()
	{
		_steamBridge.InitAuth(OnConnect);
	}

	private void OnConnect(User user)
	{
		try
		{
			base.Initialize();
			_maintenanceListener.Track();
		}
		catch (Exception ex)
		{
			_exceptionHandler.Handle(ex);
		}
	}
}

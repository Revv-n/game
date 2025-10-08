using System;
using GreenT.HornyScapes.Maintenance;
using GreenT.HornyScapes.Nutaku.Android;
using GreenT.HornyScapes.Saves;
using GreenT.Net.User;
using Nutaku.Unity;
using UniRx;

namespace GreenT.HornyScapes.Android.Nutaku;

public class EntryPoint : BaseEntryPoint, IDisposable
{
	private readonly MaintenanceListener _maintenanceListener;

	private readonly NutakuAuthorization nutakuAuthorization;

	private readonly FrameRateSetter _frameRateSetter;

	private CompositeDisposable connectStream = new CompositeDisposable();

	public EntryPoint(GameController gameController, SaveController saveController, MaintenanceListener maintenanceListener, RestoreSessionProcessor restoreSessionProcessor, NutakuAuthorization nutakuAuthorization, FrameRateSetter frameRateSetter)
		: base(gameController, saveController, restoreSessionProcessor)
	{
		_maintenanceListener = maintenanceListener;
		this.nutakuAuthorization = nutakuAuthorization;
		_frameRateSetter = frameRateSetter;
	}

	public override void Initialize()
	{
		_frameRateSetter.SetFrameRate();
		InitAuth();
	}

	private void InitAuth()
	{
		SdkPlugin.Initialize();
		ConnectAccountWithServer();
	}

	private void OnConnect()
	{
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

	private void ConnectAccountWithServer()
	{
		connectStream?.Clear();
		nutakuAuthorization.OnSuccess.Subscribe(delegate
		{
			OnConnect();
		}).AddTo(connectStream);
		nutakuAuthorization.OnFail.Subscribe(delegate(string _message)
		{
			throw new Exception(_message).LogException();
		}).AddTo(connectStream);
		Authorize();
	}

	private void Authorize()
	{
		nutakuAuthorization.ConnectToServer();
	}

	public void Dispose()
	{
		connectStream?.Dispose();
	}
}

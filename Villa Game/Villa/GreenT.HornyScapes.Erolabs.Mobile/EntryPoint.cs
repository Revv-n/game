using System;
using System.Collections;
using GreenT.HornyScapes.Maintenance;
using GreenT.HornyScapes.Saves;
using GreenT.Net.User;
using UniRx;

namespace GreenT.HornyScapes.Erolabs.Mobile;

public class EntryPoint : BaseEntryPoint, IDisposable
{
	private CompositeDisposable compositeDisposable = new CompositeDisposable();

	private readonly MaintenanceListener maintenanceListener;

	private readonly ErolabsSDKAuthorization erolabsSDKAuthorization;

	private readonly ErolabsSDKInitializer erolabsSDKInitializer;

	private readonly FrameRateSetter frameRateSetter;

	public EntryPoint(GameController gameController, SaveController saveController, MaintenanceListener maintenanceListener, RestoreSessionProcessor restoreSessionProcessor, ErolabsSDKAuthorization erolabsSDKAuthorization, FrameRateSetter frameRateSetter, ErolabsSDKInitializer erolabsSDKInitializer)
		: base(gameController, saveController, restoreSessionProcessor)
	{
		this.maintenanceListener = maintenanceListener;
		this.erolabsSDKAuthorization = erolabsSDKAuthorization;
		this.erolabsSDKInitializer = erolabsSDKInitializer;
		this.frameRateSetter = frameRateSetter;
	}

	public override void Initialize()
	{
		frameRateSetter.SetFrameRate();
		InitSDK();
	}

	private void InitSDK()
	{
		erolabsSDKInitializer.StartCoroutine(InitSDK(InitAuth));
	}

	private void InitAuth()
	{
		erolabsSDKAuthorization.OnSuccess.Subscribe(delegate
		{
			BaseInitialize();
		}).AddTo(compositeDisposable);
		erolabsSDKAuthorization.OpenLoginForm();
	}

	private void BaseInitialize()
	{
		base.Initialize();
		maintenanceListener.Track();
	}

	private IEnumerator InitSDK(Action callback)
	{
		yield return erolabsSDKInitializer.Init(callback);
	}

	public void Dispose()
	{
		compositeDisposable?.Dispose();
	}
}

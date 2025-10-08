using System;
using System.Collections;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<User>(erolabsSDKAuthorization.OnSuccess, (Action<User>)delegate
		{
			BaseInitialize();
		}), (ICollection<IDisposable>)compositeDisposable);
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
		CompositeDisposable obj = compositeDisposable;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}

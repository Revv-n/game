using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Constants;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Saves;

public sealed class SaveModeSelector : ISelector<SaveMode>, IDisposable
{
	private readonly SaveEventClusterManager clusterManager;

	private readonly IConstants<ILocker> lockerConstant;

	private readonly string switchToMainModeLocker;

	private readonly CompositeDisposable streams = new CompositeDisposable();

	public SaveModeSelector(SaveEventClusterManager clusterManager, IConstants<ILocker> lockerConstant, string switchToMainModeLocker)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.clusterManager = clusterManager;
		this.lockerConstant = lockerConstant;
		this.switchToMainModeLocker = switchToMainModeLocker;
	}

	public void Initialize()
	{
		Select(SaveMode.Tutorial);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)lockerConstant[switchToMainModeLocker].IsOpen, (Func<bool, bool>)((bool x) => x)), (Action<bool>)delegate
		{
			Select(SaveMode.Main);
		}), (ICollection<IDisposable>)streams);
	}

	public void Select(SaveMode selector)
	{
		clusterManager.ChangeMode(selector);
	}

	public void Dispose()
	{
		CompositeDisposable obj = streams;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}

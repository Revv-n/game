using System;
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
		this.clusterManager = clusterManager;
		this.lockerConstant = lockerConstant;
		this.switchToMainModeLocker = switchToMainModeLocker;
	}

	public void Initialize()
	{
		Select(SaveMode.Tutorial);
		lockerConstant[switchToMainModeLocker].IsOpen.Where((bool x) => x).Subscribe(delegate
		{
			Select(SaveMode.Main);
		}).AddTo(streams);
	}

	public void Select(SaveMode selector)
	{
		clusterManager.ChangeMode(selector);
	}

	public void Dispose()
	{
		streams?.Dispose();
	}
}

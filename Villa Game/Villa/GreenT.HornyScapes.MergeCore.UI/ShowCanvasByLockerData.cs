using System;
using GreenT.HornyScapes.Constants;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.MergeCore.UI;

public class ShowCanvasByLockerData : ShowCanvasByLocker
{
	public LockerListItem lockerData;

	private IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	private ILocker lockerModel;

	[Inject]
	private void Init(IConstants<ILocker> lockerConstant, IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory)
	{
		lockerModel = lockerFactory.Create(lockerData.UnlockType, lockerData.UnlockValue, LockerSourceType.ShowCanvasByLockerData);
	}

	protected override IObservable<bool> GetCondition()
	{
		return lockerModel.IsOpen;
	}
}

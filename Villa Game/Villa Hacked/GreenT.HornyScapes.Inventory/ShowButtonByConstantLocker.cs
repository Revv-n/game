using System;
using GreenT.HornyScapes.Constants;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Inventory;

public class ShowButtonByConstantLocker : ShowButtonByLocker
{
	public string ConstantKey = "inventory_button";

	private IConstants<ILocker> lockerConstant;

	[Inject]
	private void Init(IConstants<ILocker> lockerConstant)
	{
		this.lockerConstant = lockerConstant;
	}

	protected override IObservable<bool> GetCondition()
	{
		return (IObservable<bool>)lockerConstant[ConstantKey].IsOpen;
	}
}

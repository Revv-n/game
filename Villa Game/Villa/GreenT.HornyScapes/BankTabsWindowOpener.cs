using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Collection;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class BankTabsWindowOpener : FromTypeWindowOpener
{
	[SerializeField]
	private LayoutType _layoutType;

	private BankTabFinder bankTabFinder;

	private SignalBus signalBus;

	[Inject]
	private void Constructor(SignalBus signalBus, BankTabFinder bankTabFinder)
	{
		this.bankTabFinder = bankTabFinder;
		this.signalBus = signalBus;
	}

	public override void Click()
	{
		base.Click();
		if (bankTabFinder.TryGetActiveTab(_layoutType, out var id))
		{
			signalBus.Fire(new OpenTabSignal(id));
		}
	}
}

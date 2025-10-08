using System.Collections.Generic;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class BankSectionRedirectDispatcher
{
	private readonly IConstants<int> constants;

	private readonly SignalBus signalBus;

	private Dictionary<CurrencyType, int> tabByCurreny;

	public BankSectionRedirectDispatcher(IConstants<int> constants, SignalBus signalBus)
	{
		this.signalBus = signalBus;
		this.constants = constants;
		tabByCurreny = new Dictionary<CurrencyType, int>();
		tabByCurreny[CurrencyType.Soft] = constants["banktab_no_coins"];
		tabByCurreny[CurrencyType.Hard] = constants["banktab_no_hards"];
		if (constants.TryGetValue("banktab_no_event_currency", out var value))
		{
			tabByCurreny[CurrencyType.Event] = value;
		}
		else
		{
			tabByCurreny[CurrencyType.Event] = 100003;
		}
	}

	public void SelectTab(CurrencyType type)
	{
		int tabID = tabByCurreny[type];
		signalBus.Fire(new OpenTabSignal(tabID));
	}
}

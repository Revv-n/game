using System.Collections.Generic;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Collection;
using GreenT.HornyScapes.Constants;
using StripClub.Model;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes;

public class BankCurrencyTabsWindowOpener : FromTypeWindowOpener
{
	[SerializeField]
	private CurrencyType _currencyType;

	private IConstants<int> constants;

	private SignalBus signalBus;

	private readonly Dictionary<CurrencyType, string> _constants = new Dictionary<CurrencyType, string>
	{
		{
			CurrencyType.Soft,
			"banktab_no_coins"
		},
		{
			CurrencyType.Hard,
			"banktab_no_hards"
		}
	};

	[Inject]
	private void Constructor(SignalBus signalBus, IConstants<int> constants)
	{
		this.constants = constants;
		this.signalBus = signalBus;
	}

	public override void Click()
	{
		base.Click();
		int tabID = constants[_constants[_currencyType]];
		signalBus.Fire(new OpenTabSignal(tabID));
	}
}

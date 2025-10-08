using System.Linq;
using GreenT.HornyScapes.Bank;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Bank.UI;
using GreenT.Types;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RouletteLotRedirection : LotRedirection
{
	private SignalBus _signalBus;

	private BankTabFinder _bankTabFinder;

	[Inject]
	private void Init(SignalBus signalBus, BankTabFinder bankTabFinder)
	{
		_signalBus = signalBus;
		_bankTabFinder = bankTabFinder;
	}

	public override bool TryRedirect(CompositeIdentificator currencyIdentificator)
	{
		return false;
	}

	public override bool TryStraightRedirect(int bankTabId)
	{
		if (_bankTabFinder.GetTabs().Any((BankTab tab) => tab.ID == bankTabId))
		{
			_signalBus.Fire<OpenTabSignal>(new OpenTabSignal(bankTabId));
			return true;
		}
		return false;
	}
}

using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using GreenT.UI;
using Zenject;

namespace StripClub.Model.Shop;

public class NoCurrencyTabOpener
{
	private readonly SignalBus _signalBus;

	private readonly Dictionary<CurrencyType, int> _tabs;

	private readonly IWindowsManager _windowsManager;

	public NoCurrencyTabOpener(SignalBus signalBus, IConstants<int> intConstants, IWindowsManager windowsManager)
	{
		_signalBus = signalBus;
		_windowsManager = windowsManager;
		_tabs = new Dictionary<CurrencyType, int>
		{
			{
				CurrencyType.Hard,
				intConstants["banktab_no_hards"]
			},
			{
				CurrencyType.Soft,
				intConstants["banktab_no_coins"]
			}
		};
	}

	public void Open(CurrencyType currencyType)
	{
		if (_tabs.ContainsKey(currencyType))
		{
			BankWindow bankWindow = _windowsManager.Get<BankWindow>();
			_windowsManager.GetOpened().Last().Close();
			bankWindow.Open();
			_signalBus.Fire<OpenTabSignal>(new OpenTabSignal(_tabs[currencyType]));
		}
	}
}

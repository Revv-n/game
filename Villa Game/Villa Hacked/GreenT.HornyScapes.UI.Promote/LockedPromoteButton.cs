using GreenT.HornyScapes.Bank.UI;
using GreenT.HornyScapes.Constants;
using GreenT.UI;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.UI.Promote;

public class LockedPromoteButton : MonoBehaviour
{
	[SerializeField]
	public Button button;

	private IWindowsManager windowsManager;

	private BankWindow bankWindow;

	private SignalBus signalBus;

	private int tabIDNoSouls;

	private int tabIDNoCoins;

	private int tabIDNoCoinsAndSouls;

	protected bool isEnoughCurrency;

	protected bool isEnoughSouls;

	[Inject]
	public void Init(IWindowsManager windowsManager, SignalBus signalBus, IConstants<int> constants)
	{
		this.windowsManager = windowsManager;
		this.signalBus = signalBus;
		tabIDNoCoins = constants["banktab_no_coins"];
		tabIDNoSouls = constants["banktab_no_souls"];
		tabIDNoCoinsAndSouls = constants["banktab_no_souls_and_coins"];
	}

	public void Awake()
	{
		button.onClick.AddListener(Action);
	}

	private void OnDestroy()
	{
		button.onClick.RemoveListener(Action);
	}

	public void Action()
	{
		if (bankWindow == null)
		{
			bankWindow = windowsManager.Get<BankWindow>();
		}
		bankWindow.Open();
		int tabID = ((!isEnoughCurrency && !isEnoughSouls) ? tabIDNoCoinsAndSouls : ((!isEnoughCurrency) ? tabIDNoCoins : tabIDNoSouls));
		signalBus.Fire<OpenTabSignal>(new OpenTabSignal(tabID));
	}

	public void Set(bool isEnoughCurrency, bool isEnoughSouls)
	{
		this.isEnoughCurrency = isEnoughCurrency;
		this.isEnoughSouls = isEnoughSouls;
	}
}

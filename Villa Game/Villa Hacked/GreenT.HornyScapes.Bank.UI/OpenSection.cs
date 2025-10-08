using GreenT.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Bank.UI;

public class OpenSection : MonoBehaviour
{
	private IWindowsManager windowsManager;

	private SignalBus signalBus;

	private BankWindow bankWindow;

	[SerializeField]
	private int _sectionID;

	[Inject]
	private void Init(SignalBus signalBus, IWindowsManager windowsManager)
	{
		this.signalBus = signalBus;
		this.windowsManager = windowsManager;
	}

	public void Set(int sectionID)
	{
		_sectionID = sectionID;
	}

	public void Open()
	{
		if (bankWindow == null)
		{
			bankWindow = windowsManager.Get<BankWindow>();
		}
		bankWindow.Open();
		signalBus.Fire<OpenTabSignal>(new OpenTabSignal(_sectionID));
	}
}

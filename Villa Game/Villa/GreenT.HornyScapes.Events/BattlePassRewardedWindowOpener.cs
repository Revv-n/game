using System;
using GreenT.HornyScapes._HornyScapes._Scripts.UI.IndicatorAdapter;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class BattlePassRewardedWindowOpener
{
	private readonly IWindowsManager windowsManager;

	private readonly WindowID startWindowID;

	private readonly WindowID progressWindowID;

	private IWindow startWindow;

	private readonly SignalBus _signalBus;

	private IWindow StartWindow => startWindow ?? (startWindow = windowsManager.GetWindow(startWindowID));

	public BattlePassRewardedWindowOpener(IWindowsManager windowsManager, WindowID startWindowID, WindowID progressWindowID, SignalBus signalBus)
	{
		_signalBus = signalBus;
		this.windowsManager = windowsManager;
		this.startWindowID = startWindowID;
		this.progressWindowID = progressWindowID;
	}

	public void OpenProgress()
	{
		TryCloseStartWindow();
		windowsManager.GetWindow(progressWindowID).Open();
	}

	public void CloseProgress()
	{
		windowsManager.GetWindow(progressWindowID).Close();
	}

	private bool TryCloseStartWindow()
	{
		_signalBus.TryFire(new IndicatorSignals.PushRequest(status: false, FilteredIndicatorType.BattlePassStart));
		if (!StartWindow.IsOpened)
		{
			return false;
		}
		StartWindow.Close();
		return true;
	}

	private void OpenMeta()
	{
		TryCloseStartWindow();
	}

	public void Show(EntityStatus status)
	{
		switch (status)
		{
		case EntityStatus.Complete:
			OpenMeta();
			break;
		case EntityStatus.Rewarded:
			OpenMeta();
			break;
		default:
			throw new ArgumentOutOfRangeException(status.ToString());
		}
	}
}

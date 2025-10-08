using System;
using GreenT.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Windows.Steam;

public class SteamMonetizationPopupOpener : IInitializable, IDisposable
{
	private CompositeDisposable onBuyStream = new CompositeDisposable();

	private SteamPopup _popup;

	private readonly IWindowsManager windowsManager;

	private readonly MonetizationSystem system;

	public SteamMonetizationPopupOpener(IWindowsManager windowsManager, MonetizationSystem system)
	{
		this.windowsManager = windowsManager;
		this.system = system;
	}

	public void Initialize()
	{
		system.OnPressButton.Subscribe(delegate
		{
			DisplayPopup(display: true);
		}).AddTo(onBuyStream);
		system.OnSucceeded.Subscribe(delegate
		{
			DisplayPopup(display: false);
		}).AddTo(onBuyStream);
		system.OnFailed.Subscribe(delegate
		{
			_popup?.SetFailedView();
		}).AddTo(onBuyStream);
	}

	public void DisplayPopup(bool display)
	{
		GetWindows();
		if (display)
		{
			_popup.Open();
		}
		else
		{
			_popup.Close();
		}
	}

	private void GetWindows()
	{
		if (!_popup)
		{
			_popup = windowsManager.Get<SteamPopup>();
		}
	}

	public void Dispose()
	{
		onBuyStream?.Dispose();
	}
}

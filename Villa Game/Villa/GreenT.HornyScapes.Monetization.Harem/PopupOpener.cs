using System;
using GreenT.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Harem;

public class PopupOpener : IInitializable, IDisposable
{
	private CompositeDisposable onBuyStream = new CompositeDisposable();

	private HaremPopup haremPopup;

	private readonly IWindowsManager windowsManager;

	private readonly IIAPController<Transaction> system;

	public PopupOpener(IWindowsManager windowsManager, IIAPController<Transaction> system)
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
			haremPopup?.SetFailedView();
		}).AddTo(onBuyStream);
	}

	public void DisplayPopup(bool display)
	{
		GetWindows();
		if (display)
		{
			haremPopup.Open();
		}
		else
		{
			haremPopup.Close();
		}
	}

	private void GetWindows()
	{
		if (!haremPopup)
		{
			haremPopup = windowsManager.Get<HaremPopup>();
		}
	}

	public void Dispose()
	{
		onBuyStream?.Dispose();
	}
}

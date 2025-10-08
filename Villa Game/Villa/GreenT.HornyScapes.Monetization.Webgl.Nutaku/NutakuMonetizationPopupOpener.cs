using System;
using GreenT.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Webgl.Nutaku;

public class NutakuMonetizationPopupOpener : IInitializable, IDisposable
{
	private CompositeDisposable onBuyStream = new CompositeDisposable();

	private NutakuPopup nutakuPopup;

	private readonly IWindowsManager windowsManager;

	private readonly IIAPController<Transaction> system;

	public NutakuMonetizationPopupOpener(IWindowsManager windowsManager, IIAPController<Transaction> system)
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
			nutakuPopup?.SetFailedView();
		}).AddTo(onBuyStream);
	}

	public void DisplayPopup(bool display)
	{
		GetWindows();
		if (display)
		{
			nutakuPopup.Open();
		}
		else
		{
			nutakuPopup.Close();
		}
	}

	private void GetWindows()
	{
		if (!nutakuPopup)
		{
			nutakuPopup = windowsManager.Get<NutakuPopup>();
		}
	}

	public void Dispose()
	{
		onBuyStream?.Dispose();
	}
}

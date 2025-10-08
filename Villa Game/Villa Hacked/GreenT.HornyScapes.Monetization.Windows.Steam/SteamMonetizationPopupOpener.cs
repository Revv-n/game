using System;
using System.Collections.Generic;
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.windowsManager = windowsManager;
		this.system = system;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Product>(system.OnPressButton, (Action<Product>)delegate
		{
			DisplayPopup(display: true);
		}), (ICollection<IDisposable>)onBuyStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<SteamPaymentData>(system.OnSucceeded, (Action<SteamPaymentData>)delegate
		{
			DisplayPopup(display: false);
		}), (ICollection<IDisposable>)onBuyStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(system.OnFailed, (Action<string>)delegate
		{
			_popup?.SetFailedView();
		}), (ICollection<IDisposable>)onBuyStream);
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
		CompositeDisposable obj = onBuyStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}

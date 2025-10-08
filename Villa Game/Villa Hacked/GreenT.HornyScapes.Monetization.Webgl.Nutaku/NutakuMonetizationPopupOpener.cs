using System;
using System.Collections.Generic;
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Transaction>(system.OnSucceeded, (Action<Transaction>)delegate
		{
			DisplayPopup(display: false);
		}), (ICollection<IDisposable>)onBuyStream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(system.OnFailed, (Action<string>)delegate
		{
			nutakuPopup?.SetFailedView();
		}), (ICollection<IDisposable>)onBuyStream);
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
		CompositeDisposable obj = onBuyStream;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}

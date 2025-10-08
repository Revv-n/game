using System;
using System.Collections.Generic;
using GreenT.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Monetization.Android.Erolabs;

public class ErolabsMonetizationPopupOpener : IInitializable, IDisposable
{
	private readonly IWindowsManager windowsManager;

	private readonly MonetizationSystem monetizationSystem;

	private ErolabsPopup popup;

	private CompositeDisposable compositeDisposable = new CompositeDisposable();

	public ErolabsMonetizationPopupOpener(IWindowsManager windowsManager, MonetizationSystem monetizationSystem)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		this.windowsManager = windowsManager;
		this.monetizationSystem = monetizationSystem;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Product>(monetizationSystem.OnPressButton, (Action<Product>)delegate
		{
			DisplayPopup(display: true);
		}), (ICollection<IDisposable>)compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Transaction>(monetizationSystem.OnSucceeded, (Action<Transaction>)delegate
		{
			DisplayPopup(display: false);
		}), (ICollection<IDisposable>)compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<string>(monetizationSystem.OnFailed, (Action<string>)delegate
		{
			popup?.SetFailedView();
		}), (ICollection<IDisposable>)compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(monetizationSystem.OnNotEnoughMoney, (Action<Unit>)delegate
		{
			popup?.ShowNotEnoughMoneyView();
		}), (ICollection<IDisposable>)compositeDisposable);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Tuple<bool, int, Action<Unit>, bool>>(monetizationSystem.OnStartBuyButton, (Action<Tuple<bool, int, Action<Unit>, bool>>)delegate(Tuple<bool, int, Action<Unit>, bool> purchaseData)
		{
			if (purchaseData.Item4)
			{
				popup?.ShowGuestView();
			}
			else if (purchaseData.Item1)
			{
				popup?.ShowCheckBalanceView(purchaseData.Item2, purchaseData.Item3);
			}
			else
			{
				popup?.ShowNotEnoughMoneyView();
			}
		}), (ICollection<IDisposable>)compositeDisposable);
	}

	public void ForceShowBinding()
	{
		DisplayPopup(display: true);
		popup.ShowBindingView();
	}

	public void ForceShowBindComplete()
	{
		popup.ShowBindComplete();
	}

	public void DisplayPopup(bool display)
	{
		GetWindows();
		if (display)
		{
			popup.Open();
		}
		else
		{
			popup.Close();
		}
	}

	private void GetWindows()
	{
		if (!popup)
		{
			popup = windowsManager.Get<ErolabsPopup>();
		}
	}

	public void Dispose()
	{
		CompositeDisposable obj = compositeDisposable;
		if (obj != null)
		{
			obj.Dispose();
		}
	}
}

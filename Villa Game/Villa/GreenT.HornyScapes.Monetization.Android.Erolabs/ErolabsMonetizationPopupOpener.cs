using System;
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
		this.windowsManager = windowsManager;
		this.monetizationSystem = monetizationSystem;
	}

	public void Initialize()
	{
		monetizationSystem.OnPressButton.Subscribe(delegate
		{
			DisplayPopup(display: true);
		}).AddTo(compositeDisposable);
		monetizationSystem.OnSucceeded.Subscribe(delegate
		{
			DisplayPopup(display: false);
		}).AddTo(compositeDisposable);
		monetizationSystem.OnFailed.Subscribe(delegate
		{
			popup?.SetFailedView();
		}).AddTo(compositeDisposable);
		monetizationSystem.OnNotEnoughMoney.Subscribe(delegate
		{
			popup?.ShowNotEnoughMoneyView();
		}).AddTo(compositeDisposable);
		monetizationSystem.OnStartBuyButton.Subscribe(delegate(Tuple<bool, int, Action<Unit>, bool> purchaseData)
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
		}).AddTo(compositeDisposable);
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
		compositeDisposable?.Dispose();
	}
}

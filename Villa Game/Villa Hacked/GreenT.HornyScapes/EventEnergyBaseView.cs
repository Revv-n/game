using System;
using StripClub.Model;
using StripClub.Model.Shop;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class EventEnergyBaseView : MonoBehaviour
{
	[Header("Buy option:")]
	[SerializeField]
	protected Button buyButton;

	private NoCurrencyTabOpener _noCurrencyTabOpener;

	[Inject]
	private void InnerInit(NoCurrencyTabOpener noCurrencyTabOpener)
	{
		_noCurrencyTabOpener = noCurrencyTabOpener;
	}

	protected void SetupActionButton()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityUIComponentExtensions.OnClickAsObservable(buyButton), (Action<Unit>)delegate
		{
			TryPurchase();
		}), (Component)this);
	}

	protected virtual void TryPurchase()
	{
		OpenBank();
	}

	protected void OpenBank()
	{
		_noCurrencyTabOpener.Open(CurrencyType.Hard);
	}
}

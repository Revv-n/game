using System.Linq;
using GreenT.HornyScapes.Resources.UI;
using GreenT.HornyScapes.Sellouts.Models;
using GreenT.HornyScapes.Sellouts.Services;
using GreenT.Model.Reactive;
using GreenT.UI;
using StripClub.Model;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Sellouts.Windows;

public class SelloutWindow : Window, IInitializable
{
	[SerializeField]
	private SelloutUiSetter _selloutUiSetter;

	[SerializeField]
	private Button _closeButton;

	private GreenT.Model.Reactive.ReactiveCollection<CurrencyType> _visibleCurrenciesManager;

	private SelloutStateManager _selloutStateManager;

	private CurrencyType[] _previousWindowsVisibleCurrencies;

	[Inject]
	private void Init(GreenT.Model.Reactive.ReactiveCollection<CurrencyType> manager, SelloutStateManager selloutStateManager)
	{
		_visibleCurrenciesManager = manager;
		_selloutStateManager = selloutStateManager;
	}

	public void Initialize()
	{
		_selloutStateManager.Activated.Subscribe(delegate(Sellout sellout)
		{
			Set(sellout);
		}).AddTo(this);
	}

	public override void Open()
	{
		if (!IsOpened)
		{
			_previousWindowsVisibleCurrencies = _visibleCurrenciesManager.ToArray();
			_visibleCurrenciesManager.SetItems(CurrencyType.Soft, CurrencyType.Hard);
		}
		base.Open();
		windowsManager.Get<ResourcesWindow>().Open();
	}

	public override void Close()
	{
		_visibleCurrenciesManager.SetItems(_previousWindowsVisibleCurrencies);
		base.Close();
		windowsManager.Get<ResourcesWindow>().Open();
	}

	private void OnEnable()
	{
		_closeButton.OnClickAsObservable().TakeUntilDisable(this).Subscribe(delegate
		{
			Close();
		})
			.AddTo(this);
	}

	private void Set(Sellout sellout)
	{
		_selloutUiSetter.Set(sellout);
	}
}

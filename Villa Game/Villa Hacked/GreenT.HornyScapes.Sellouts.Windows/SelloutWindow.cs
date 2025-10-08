using System;
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

	private ReactiveCollection<CurrencyType> _visibleCurrenciesManager;

	private SelloutStateManager _selloutStateManager;

	private CurrencyType[] _previousWindowsVisibleCurrencies;

	[Inject]
	private void Init(ReactiveCollection<CurrencyType> manager, SelloutStateManager selloutStateManager)
	{
		_visibleCurrenciesManager = manager;
		_selloutStateManager = selloutStateManager;
	}

	public void Initialize()
	{
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Sellout>(_selloutStateManager.Activated, (Action<Sellout>)delegate(Sellout sellout)
		{
			Set(sellout);
		}), (Component)this);
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
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(Observable.TakeUntilDisable<Unit>(UnityUIComponentExtensions.OnClickAsObservable(_closeButton), (Component)this), (Action<Unit>)delegate
		{
			Close();
		}), (Component)this);
	}

	private void Set(Sellout sellout)
	{
		_selloutUiSetter.Set(sellout);
	}
}

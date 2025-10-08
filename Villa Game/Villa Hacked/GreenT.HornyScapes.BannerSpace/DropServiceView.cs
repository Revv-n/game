using System;
using System.Collections.Generic;
using GreenT.UI;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class DropServiceView : MonoView<Banner>
{
	[SerializeField]
	private CurrencyColorObserver _price1;

	[SerializeField]
	private Button _price1Button;

	[SerializeField]
	private CurrencyColorObserver _price10;

	[SerializeField]
	private Button _price10Button;

	[SerializeField]
	private LocalizedTextMeshPro _forGrantChance;

	private DropService _dropService;

	private readonly CompositeDisposable _stream = new CompositeDisposable();

	private ICurrencyProcessor _currencyProcessor;

	private IWindowsManager _windowsManager;

	private BannerResourcesWindow _bannerResourcesWindow;

	[Inject]
	private void Initialization(DropService dropService, ICurrencyProcessor currencyProcessor, IWindowsManager windowsManager)
	{
		_windowsManager = windowsManager;
		_currencyProcessor = currencyProcessor;
		_dropService = dropService;
	}

	public override void Set(Banner banner)
	{
		CompositeDisposable stream = _stream;
		if (stream != null)
		{
			stream.Clear();
		}
		base.Set(banner);
		UpdateBannerResourcesWindowInfo();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<int>(Observable.Select<int, int>(Observable.Where<int>(_dropService.OnGrantChange, (Func<int, bool>)((int id) => id == banner.Id)), (Func<int, int>)((int id) => GetCount())), (Action<int>)UpdateText), (ICollection<IDisposable>)_stream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityEventExtensions.AsObservable((UnityEvent)_price1Button.onClick), (Action<Unit>)delegate
		{
			TryBuy(1, banner.BuyPrice);
		}), (ICollection<IDisposable>)_stream);
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Unit>(UnityEventExtensions.AsObservable((UnityEvent)_price10Button.onClick), (Action<Unit>)delegate
		{
			TryBuy(10, banner.BuyPrice10);
		}), (ICollection<IDisposable>)_stream);
		UpdateText(GetCount());
		_price1.Set(banner.BuyPrice);
		_price10.Set(banner.BuyPrice10);
	}

	private void OnEnable()
	{
		if (base.Source != null)
		{
			UpdateBannerResourcesWindowInfo();
		}
	}

	private void UpdateBannerResourcesWindowInfo()
	{
		if ((object)_bannerResourcesWindow == null)
		{
			_bannerResourcesWindow = _windowsManager.Get<BannerResourcesWindow>();
		}
		_bannerResourcesWindow.Set(base.Source);
	}

	private void TryBuy(int count, Price<int> price)
	{
		if (_currencyProcessor.TrySpent(price.Currency, price.Value, price.CompositeIdentificator))
		{
			_dropService.Drop(base.Source, count, price);
			return;
		}
		int count2 = _currencyProcessor.GetCount(price.Currency, price.CompositeIdentificator);
		_bannerResourcesWindow.Set(base.Source);
		_bannerResourcesWindow.SetCount(price.Value - count2);
		_bannerResourcesWindow.Open();
	}

	private void UpdateText(int count)
	{
		_forGrantChance.SetArguments(count);
	}

	private int GetCount()
	{
		return _dropService.GetDifferenceBetweenSteps(base.Source);
	}
}

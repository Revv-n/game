using GreenT.UI;
using StripClub.Model.Shop;
using StripClub.UI;
using UniRx;
using UnityEngine;
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
		_stream?.Clear();
		base.Set(banner);
		UpdateBannerResourcesWindowInfo();
		(from id in _dropService.OnGrantChange
			where id == banner.Id
			select GetCount()).Subscribe(UpdateText).AddTo(_stream);
		_price1Button.onClick.AsObservable().Subscribe(delegate
		{
			TryBuy(1, banner.BuyPrice);
		}).AddTo(_stream);
		_price10Button.onClick.AsObservable().Subscribe(delegate
		{
			TryBuy(10, banner.BuyPrice10);
		}).AddTo(_stream);
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

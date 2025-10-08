using System;
using System.Linq;
using GreenT.HornyScapes.Bank.BankTabs;
using GreenT.HornyScapes.Events.Content;
using GreenT.Model.Reactive;
using GreenT.Types;
using StripClub.Model;
using StripClub.UI.Shop;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.BannerSpace;

public class BannerView : BankSectionView
{
	[SerializeField]
	private InfoWindow _infoWindow;

	[SerializeField]
	private MainRewardInfoView _mainRewardInfoView;

	[SerializeField]
	private DropServiceView _dropServiceView;

	private BannerBackgroundContainer _backgroundContainer;

	private BannerAnimatedBackground _bannerAnimatedBackground;

	private IBannerCluster _bannerCluster;

	private ContentSelectorGroup _contentSelectorGroup;

	private Banner _banner;

	private ReactiveCollection<CurrencyType> _visibleCurrencyTypes;

	private CurrencyType[] _prevWindowsVisibleCurrencies = Array.Empty<CurrencyType>();

	private CurrencyType[] _targetCurrencies = Array.Empty<CurrencyType>();

	[Inject]
	public void SetStoreService(BannerBackgroundContainer background, IBannerCluster bannerCluster, ContentSelectorGroup contentSelectorGroup, ReactiveCollection<CurrencyType> visibleCurrencyTypes)
	{
		_backgroundContainer = background;
		_bannerCluster = bannerCluster;
		_contentSelectorGroup = contentSelectorGroup;
		_visibleCurrencyTypes = visibleCurrencyTypes;
	}

	public override void Set(BankTab settings)
	{
		base.Set(settings);
		ContentType current = _contentSelectorGroup.Current;
		Banner forTab = _bannerCluster.GetForTab(current, settings.ID);
		if (forTab == _banner)
		{
			if (_backgroundContainer.StaticBackground.sprite != null)
			{
				_backgroundContainer.gameObject.SetActive(value: true);
			}
			return;
		}
		_banner = forTab;
		_targetCurrencies = new CurrencyType[2]
		{
			_banner.RebuyCost.Currency,
			_banner.BuyPrice.Currency
		};
		SetVisibleCurrency();
		SetBackground();
		SetMainRewardInfo();
		_infoWindow.Set(forTab);
		_dropServiceView.Set(forTab);
	}

	private void SetBackground()
	{
		if (_bannerAnimatedBackground != null)
		{
			UnityEngine.Object.Destroy(_bannerAnimatedBackground.gameObject);
		}
		if (_banner.Background.AnimatedBackground != null)
		{
			_bannerAnimatedBackground = UnityEngine.Object.Instantiate(_banner.Background.AnimatedBackground, _backgroundContainer.transform);
			return;
		}
		_backgroundContainer.gameObject.SetActive(value: true);
		_backgroundContainer.StaticBackground.sprite = _banner.Background.Base;
		_backgroundContainer.StaticBackground.gameObject.SetActive(value: true);
	}

	private void SetMainRewardInfo()
	{
		LinkedContent linkedContent = _banner.LegendaryRewardInfos.FirstOrDefault((RewardInfo x) => x.IsMain)?.LinkedContent;
		_mainRewardInfoView.Set(linkedContent, _banner.Background.TextPosition);
	}

	private void OnEnable()
	{
		if (_bannerAnimatedBackground != null)
		{
			_backgroundContainer.gameObject.SetActive(value: true);
		}
		else
		{
			_backgroundContainer.StaticBackground.gameObject.SetActive(value: true);
		}
		if (_banner != null)
		{
			SetVisibleCurrency();
		}
	}

	private void SetVisibleCurrency()
	{
		_prevWindowsVisibleCurrencies = _visibleCurrencyTypes.ToArray();
		_visibleCurrencyTypes.SetItems(_targetCurrencies);
	}

	protected override void OnDisable()
	{
		if (_backgroundContainer != null)
		{
			_backgroundContainer.gameObject.SetActive(value: false);
		}
		if (_visibleCurrencyTypes.ToArray().SequenceEqual(_targetCurrencies))
		{
			_visibleCurrencyTypes.SetItems(_prevWindowsVisibleCurrencies);
		}
		_infoWindow.Display(display: false);
		_backgroundContainer.StaticBackground.gameObject.SetActive(value: false);
		base.OnDisable();
	}

	private void OnDestroy()
	{
		if (_bannerAnimatedBackground != null && _bannerAnimatedBackground.gameObject != null)
		{
			UnityEngine.Object.Destroy(_bannerAnimatedBackground.gameObject);
		}
	}
}

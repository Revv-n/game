using System;
using System.Linq;
using GreenT.HornyScapes.Animations;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Resources.UI;
using GreenT.Model.Reactive;
using GreenT.Types;
using StripClub.Model;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventWindowView : PopupWindow
{
	[SerializeField]
	private LocalizedTextMeshPro _title;

	[SerializeField]
	private LocalizedTextMeshPro _titleShadow;

	[SerializeField]
	private GameObject _tabbedRoot;

	[SerializeField]
	private GameObject _notabsRoot;

	[SerializeField]
	private Image _girl;

	[SerializeField]
	private Image _background;

	[SerializeField]
	private CurrencyView _currencyView;

	private MiniEventViewControllerService _miniEventViewControllerService;

	private MiniEventsBundlesProvider _miniBundlesProvider;

	private GreenT.Model.Reactive.ReactiveCollection<CurrencyType> _visibleCurrenciesManager;

	private bool _isFirstShow = true;

	private IDisposable _emptyTabsTracker;

	private CurrencyType[] _previousWindowsVisibleCurrencies;

	private readonly string TITLE_KEY = "ui.minievent.event_title_{0}.name";

	private BundlesProviderBase _bundlesProvider;

	[Inject]
	private void Init(MiniEventViewControllerService miniEventViewControllerProvider, BundlesProviderBase bundlesProvider, MiniEventsBundlesProvider miniBundlesProvider, GreenT.Model.Reactive.ReactiveCollection<CurrencyType> visibleCurrenciesManager)
	{
		_miniEventViewControllerService = miniEventViewControllerProvider;
		_miniBundlesProvider = miniBundlesProvider;
		_visibleCurrenciesManager = visibleCurrenciesManager;
		_bundlesProvider = bundlesProvider;
	}

	private void Start()
	{
		_emptyTabsTracker = (from isAnyActive in _miniEventViewControllerService.IsAnyActiveMiniEventReactive.Skip(1)
			where !isAnyActive
			select isAnyActive).Subscribe(delegate
		{
			ForceClose();
		});
	}

	private void OnEnable()
	{
		_miniEventViewControllerService.TryRemoveEmptyTabs();
		if (_isFirstShow)
		{
			_isFirstShow = false;
			_miniEventViewControllerService.InteractFirstMiniEvent();
		}
		_miniEventViewControllerService.HandleCurrentTabContent();
	}

	public override void Open()
	{
		if (!IsOpened)
		{
			_previousWindowsVisibleCurrencies = _visibleCurrenciesManager.ToArray();
			_visibleCurrenciesManager.SetItems(CurrencyType.Soft, CurrencyType.Hard);
		}
		base.Open();
		OpenResourceWindow();
	}

	public override void Close()
	{
		_visibleCurrenciesManager.SetItems(_previousWindowsVisibleCurrencies);
		base.Close();
		OpenResourceWindow();
	}

	public void InteractMiniEventView(CompositeIdentificator eventIdentificator, int eventId, bool isMultiTabbed, CompositeIdentificator currencyIdentificator, ConfigType configType)
	{
		if (_miniEventViewControllerService.InteractMiniEvent(eventIdentificator, isForced: true))
		{
			_miniEventViewControllerService.HandleActivityTabs(eventIdentificator, isMultiTabbed);
			UpdateTitle(eventId);
			SetTabState(isMultiTabbed);
			SetBackground();
			_currencyView.gameObject.SetActive(currencyIdentificator[0] != 0 && configType != ConfigType.Rating);
			_currencyView.Setup(CurrencyType.MiniEvent, currencyIdentificator);
		}
	}

	public void InteractTabView(CompositeIdentificator tabIdentificator, TabType tabType, bool isMultiTabbed = true)
	{
		_miniEventViewControllerService.HandleTabContent(tabIdentificator, tabType, isMultiTabbed);
	}

	public void HandleEmptyActivityTab()
	{
		_miniEventViewControllerService.HandleEmptyActivityTab();
	}

	private void UpdateTitle(int eventId)
	{
		string key = string.Format(TITLE_KEY, eventId);
		_title.Init(key);
		_titleShadow.Init(key);
	}

	private void SetTabState(bool isMultiTabbed)
	{
		_tabbedRoot.SetActive(isMultiTabbed);
		_notabsRoot.SetActive(!isMultiTabbed);
	}

	private void SetBackground()
	{
		MiniEvent currentMiniEvent = _miniEventViewControllerService.CurrentMiniEvent;
		Sprite sprite = null;
		if (currentMiniEvent.ViewSetting.HasValue)
		{
			sprite = currentMiniEvent.ViewSetting.Value.Background;
			if (currentMiniEvent.ViewSetting.Value.Girl != null)
			{
				_girl.sprite = currentMiniEvent.ViewSetting.Value.Girl;
			}
			else
			{
				_girl.color = Color.clear;
			}
		}
		else
		{
			MiniEventBundleData miniEventBundleData = _miniBundlesProvider.TryGet(currentMiniEvent.EventId).LoadAllAssets<MiniEventBundleData>().FirstOrDefault();
			if (miniEventBundleData != null)
			{
				sprite = miniEventBundleData.ContentBackground;
			}
			_girl.color = Color.clear;
		}
		_background.sprite = sprite;
	}

	private void ForceClose()
	{
		Close();
		_isFirstShow = true;
	}

	private void OpenResourceWindow()
	{
		windowsManager.Get<ResourcesWindow>().Open();
	}

	protected override void OnDestroy()
	{
		_emptyTabsTracker?.Dispose();
	}
}

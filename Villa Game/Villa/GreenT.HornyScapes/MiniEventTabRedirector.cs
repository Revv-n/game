using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.MiniEvents;
using GreenT.UI;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class MiniEventTabRedirector
{
	private MiniEventSettingsProvider _settingsProvider;

	private MiniEventTabsManager _miniEventTabsManager;

	private MiniEventMapperManager _miniEventMapperManager;

	private ActivitiesShopManager _activitiesShopManager;

	private ActivitiesManager _activitiesManager;

	private IWindowsManager _windowsManager;

	private MiniEventWindowView _miniEventWindowView;

	[Inject]
	private void Init(MiniEventSettingsProvider settingsProvider, MiniEventTabsManager miniEventTabsManager, ActivitiesShopManager activitiesShopManager, IWindowsManager windowsManager, MiniEventMapperManager miniEventMapperManager, ActivitiesManager activitiesManager)
	{
		_settingsProvider = settingsProvider;
		_miniEventTabsManager = miniEventTabsManager;
		_activitiesShopManager = activitiesShopManager;
		_windowsManager = windowsManager;
		_miniEventMapperManager = miniEventMapperManager;
		_activitiesManager = activitiesManager;
	}

	public void RedirectRoulette(int rouletteId)
	{
		if (_miniEventWindowView == null)
		{
			_miniEventWindowView = _windowsManager.Get<MiniEventWindowView>();
		}
		ActivityShopMapper shopTab = _activitiesShopManager.Collection.FirstOrDefault((ActivityShopMapper shopTab) => shopTab.bank_tab_id == rouletteId && shopTab.tab_type == ShopTabType.Roulette);
		if (shopTab == null)
		{
			return;
		}
		ActivityMapper activity = _activitiesManager.Collection.FirstOrDefault((ActivityMapper activity) => activity.shop_tabs.Any((int id) => id == shopTab.tab_id));
		MiniEventMapper miniEventMapper = _miniEventMapperManager.Collection.FirstOrDefault((MiniEventMapper miniMapper) => miniMapper.activity_id == activity.id);
		if (miniEventMapper == null)
		{
			return;
		}
		MiniEvent minievent = _settingsProvider.Collection.FirstOrDefault((MiniEvent miniEvent) => miniEvent.EventId == miniEventMapper.id);
		if (minievent != null)
		{
			MiniEventActivityTab miniEventActivityTab = _miniEventTabsManager.Collection.FirstOrDefault((MiniEventActivityTab tab) => minievent.EventId == tab.EventIdentificator[0]);
			if (miniEventActivityTab != null && miniEventActivityTab != null)
			{
				_miniEventWindowView.InteractMiniEventView(minievent.Identificator, minievent.EventId, minievent.IsMultiTabbed, minievent.CurrencyIdentificator, minievent.ConfigType);
				_miniEventWindowView.InteractTabView(miniEventActivityTab.Identificator, miniEventActivityTab.TabType, isMultiTabbed: false);
			}
		}
	}
}

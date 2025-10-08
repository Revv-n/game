using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.HornyScapes.MiniEvents;
using GreenT.Types;
using GreenT.UI;
using StripClub.Model;
using StripClub.Model.Quest;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class MiniEventLotRedirection : LotRedirection
{
	private MiniEventSettingsProvider _settingsProvider;

	private MiniEventTabsManager _miniEventTabsManager;

	private MiniEventTaskManager _taskManager;

	private ActivitiesShopManager _activitiesShopManager;

	private IManager<Lot> _lotManager;

	private IWindowsManager _windowsManager;

	private MiniEventWindowView _miniEventWindowView;

	[Inject]
	private void Init(MiniEventSettingsProvider settingsProvider, MiniEventTabsManager miniEventTabsManager, MiniEventTaskManager taskManager, ActivitiesShopManager activitiesShopManager, IManager<Lot> lotManager, IWindowsManager windowsManager)
	{
		_settingsProvider = settingsProvider;
		_miniEventTabsManager = miniEventTabsManager;
		_taskManager = taskManager;
		_activitiesShopManager = activitiesShopManager;
		_lotManager = lotManager;
		_windowsManager = windowsManager;
	}

	public override bool TryRedirect(CompositeIdentificator currencyIdentificator)
	{
		if (_miniEventWindowView == null)
		{
			_miniEventWindowView = _windowsManager.Get<MiniEventWindowView>();
		}
		foreach (MiniEvent miniEvent in _settingsProvider.Collection.Where((MiniEvent miniEvent) => miniEvent.CurrencyIdentificator == currencyIdentificator))
		{
			IEnumerable<MiniEventActivityTab> source = _miniEventTabsManager.Collection.Where((MiniEventActivityTab tab) => miniEvent.EventId == tab.EventIdentificator[0]);
			IEnumerable<MiniEventActivityTab> tabs = source.Where((MiniEventActivityTab tab) => tab.TabType == TabType.Task);
			if (TryFindDirectCurrencyRewardTasks(tabs, currencyIdentificator, miniEvent))
			{
				return true;
			}
			if (TryFindLootboxCurrencyRewardTasks(tabs, currencyIdentificator, miniEvent))
			{
				return true;
			}
			IEnumerable<MiniEventActivityTab> tabs2 = source.Where((MiniEventActivityTab tab) => tab.TabType == TabType.Shop);
			if (TryFindLots(tabs2, ShopTabType.Offer, currencyIdentificator, miniEvent))
			{
				return true;
			}
			if (TryFindLots(tabs2, ShopTabType.BundlesList, currencyIdentificator, miniEvent))
			{
				return true;
			}
		}
		return false;
	}

	public override bool TryStraightRedirect(int bankTabId)
	{
		return false;
	}

	private bool TryFindLots(IEnumerable<MiniEventActivityTab> tabs, ShopTabType targetTabType, CompositeIdentificator currencyIdentificator, MiniEvent miniEvent)
	{
		foreach (MiniEventActivityTab tab in tabs)
		{
			if (!_activitiesShopManager.Collection.Any((ActivityShopMapper activity) => activity.bank_tab_id == tab.DataIdentificator[0] && activity.tab_type == targetTabType))
			{
				continue;
			}
			IEnumerable<Lot> enumerable = _lotManager.Collection.Where((Lot lot) => lot.TabID == tab.DataIdentificator[0] && lot.Locker.IsOpen.Value);
			if (enumerable == null || !enumerable.Any())
			{
				continue;
			}
			foreach (Lot item in enumerable)
			{
				if ((item as BundleLot).Content is LootboxLinkedContent lootboxLinkedContent && (lootboxLinkedContent.Lootbox.GuarantedDrop.Any((DropSettings drop) => drop.Selector is CurrencySelector { Currency: CurrencyType.MiniEvent } currencySelector2 && currencySelector2.Identificator == currencyIdentificator) || lootboxLinkedContent.Lootbox.DropOptions.Any((RandomDropSettings drop) => drop.Selector is CurrencySelector { Currency: CurrencyType.MiniEvent } currencySelector && currencySelector.Identificator == currencyIdentificator)))
				{
					_miniEventWindowView.InteractMiniEventView(miniEvent.Identificator, miniEvent.EventId, miniEvent.IsMultiTabbed, miniEvent.CurrencyIdentificator, miniEvent.ConfigType);
					_miniEventWindowView.InteractTabView(tab.Identificator, tab.TabType);
					return true;
				}
			}
		}
		return false;
	}

	private bool TryFindDirectCurrencyRewardTasks(IEnumerable<MiniEventActivityTab> tabs, CompositeIdentificator currencyIdentificator, MiniEvent miniEvent)
	{
		foreach (MiniEventActivityTab tab in tabs)
		{
			if (_taskManager.Tasks.Where((MiniEventTask task) => task.Identificator == tab.DataIdentificator && (task.State == StateType.Complete || task.State == StateType.Active)).FirstOrDefault((MiniEventTask task) => task.Reward is CurrencyLinkedContent { Currency: CurrencyType.MiniEvent } currencyLinkedContent && currencyLinkedContent.CompositeIdentificator == currencyIdentificator) != null)
			{
				_miniEventWindowView.InteractMiniEventView(miniEvent.Identificator, miniEvent.EventId, miniEvent.IsMultiTabbed, miniEvent.CurrencyIdentificator, miniEvent.ConfigType);
				_miniEventWindowView.InteractTabView(tab.Identificator, tab.TabType);
				return true;
			}
		}
		return false;
	}

	private bool TryFindLootboxCurrencyRewardTasks(IEnumerable<MiniEventActivityTab> tabs, CompositeIdentificator currencyIdentificator, MiniEvent miniEvent)
	{
		foreach (MiniEventActivityTab tab in tabs)
		{
			if (_taskManager.Tasks.Where((MiniEventTask task) => task.Identificator == tab.DataIdentificator && (task.State == StateType.Complete || task.State == StateType.Active)).FirstOrDefault((MiniEventTask task) => task.Reward is LootboxLinkedContent lootboxLinkedContent && (lootboxLinkedContent.Lootbox.GuarantedDrop.Any((DropSettings drop) => drop.Selector is CurrencySelector { Currency: CurrencyType.MiniEvent } currencySelector2 && currencySelector2.Identificator == currencyIdentificator) || lootboxLinkedContent.Lootbox.DropOptions.Any((RandomDropSettings drop) => drop.Selector is CurrencySelector { Currency: CurrencyType.MiniEvent } currencySelector && currencySelector.Identificator == currencyIdentificator))) != null)
			{
				_miniEventWindowView.InteractMiniEventView(miniEvent.Identificator, miniEvent.EventId, miniEvent.IsMultiTabbed, miniEvent.CurrencyIdentificator, miniEvent.ConfigType);
				_miniEventWindowView.InteractTabView(tab.Identificator, tab.TabType);
				return true;
			}
		}
		return false;
	}
}

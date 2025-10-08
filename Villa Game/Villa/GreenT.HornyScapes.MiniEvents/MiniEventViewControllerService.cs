using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventViewControllerService : IInitializable, IDisposable
{
	private readonly MiniEventTabsManager _tabsManager;

	private readonly MiniEventsStateService _miniEventsStateService;

	private readonly MiniEventViewController _miniEventController;

	private readonly MiniEventActivityTabsViewController _miniEventActivityTabsViewController;

	private readonly MiniEventRatingRootViewController _miniEventRatingRootViewController;

	private readonly MiniEventTasksRootViewController _miniEventTasksRootViewController;

	private readonly MiniEventShopRootViewControllerService _miniEventShopRootViewControllerService;

	private readonly MiniEventLotBoughtHandler _miniEventLotBoughtHandler;

	private readonly Dictionary<TabType, IContentable> _contentables;

	private IViewController _lastContentViewController;

	private (CompositeIdentificator, TabType, bool) _currentTab;

	private CompositeIdentificator _currentOpenedMinievent;

	private Dictionary<int, CompositeIdentificator> _currentMinievents;

	private IDisposable _startTracker;

	private IDisposable _endTracker;

	public MiniEvent CurrentMiniEvent { get; private set; }

	public ReactiveProperty<bool> IsAnyActiveMiniEventReactive { get; private set; }

	public MiniEventViewControllerService(MiniEventTabsManager tabsManager, MiniEventViewController miniEventController, MiniEventActivityTabsViewController miniEventTabsViewController, MiniEventTasksRootViewController miniEventTaskTabContentViewController, MiniEventShopRootViewControllerService miniEventShopRootViewControllerService, MiniEventRatingRootViewController miniEventRatingRootViewController, MiniEventsStateService miniEventsStateService, MiniEventLotBoughtHandler miniEventLotBoughtHandler)
	{
		_tabsManager = tabsManager;
		_miniEventsStateService = miniEventsStateService;
		_miniEventController = miniEventController;
		_miniEventActivityTabsViewController = miniEventTabsViewController;
		_miniEventTasksRootViewController = miniEventTaskTabContentViewController;
		_miniEventShopRootViewControllerService = miniEventShopRootViewControllerService;
		_miniEventRatingRootViewController = miniEventRatingRootViewController;
		_miniEventLotBoughtHandler = miniEventLotBoughtHandler;
		IsAnyActiveMiniEventReactive = new ReactiveProperty<bool>();
		_currentMinievents = new Dictionary<int, CompositeIdentificator>();
		_contentables = new Dictionary<TabType, IContentable>
		{
			{
				TabType.Task,
				_miniEventTasksRootViewController
			},
			{
				TabType.Shop,
				_miniEventShopRootViewControllerService
			},
			{
				TabType.Rating,
				_miniEventRatingRootViewController
			}
		};
	}

	public void Initialize()
	{
		_startTracker = _miniEventsStateService.OnStart().Subscribe(OnMiniEventStart);
		_endTracker = _miniEventsStateService.OnEnd().Subscribe(OnMiniEventEnd);
		_miniEventLotBoughtHandler.Init(HandleCurrentTabContent);
	}

	public void Dispose()
	{
		_startTracker?.Dispose();
		_endTracker?.Dispose();
		_currentMinievents?.Clear();
	}

	public bool InteractMiniEvent(CompositeIdentificator eventIdentificator, bool isForced = false)
	{
		_miniEventController.HandlePress(eventIdentificator);
		CurrentMiniEvent = _miniEventController.GetView(eventIdentificator).Source;
		_currentOpenedMinievent = eventIdentificator;
		if (isForced)
		{
			TryRemoveEmptyTabs();
			CalculateAnyActiveMiniEvent();
		}
		bool num = _miniEventController.IsActive(eventIdentificator);
		if (!num)
		{
			InteractFirstMiniEvent();
		}
		return num;
	}

	public void InteractFirstMiniEvent()
	{
		CurrentMiniEvent = _miniEventController.ShowFirst();
		CalculateAnyActiveMiniEvent();
	}

	public void HandleActivityTabs(CompositeIdentificator eventIdentificator, bool isMultiTabbed)
	{
		_miniEventActivityTabsViewController.HideAll();
		if (isMultiTabbed)
		{
			_miniEventActivityTabsViewController.Show(eventIdentificator);
		}
		HandleFirstActivityTab(eventIdentificator, isMultiTabbed);
	}

	public void HandleTabContent(CompositeIdentificator tabIdentificator, TabType tabType, bool isMultiTabbed)
	{
		_miniEventActivityTabsViewController.HandlePress(tabIdentificator, tabType);
		_lastContentViewController?.HideAll();
		MiniEventActivityTab miniEventActivityTab = _tabsManager.Collection.FirstOrDefault((MiniEventActivityTab t) => t.Identificator == tabIdentificator && t.TabType == tabType);
		CompositeIdentificator identificator = default(CompositeIdentificator);
		switch (tabType)
		{
		case TabType.Task:
			_lastContentViewController = _miniEventTasksRootViewController;
			identificator = miniEventActivityTab.DataIdentificator;
			break;
		case TabType.Shop:
			_lastContentViewController = _miniEventShopRootViewControllerService;
			identificator = miniEventActivityTab.DataIdentificator;
			break;
		case TabType.Rating:
			_lastContentViewController = _miniEventRatingRootViewController;
			identificator = new CompositeIdentificator(miniEventActivityTab.CalendarId, miniEventActivityTab.EventIdentificator[0], miniEventActivityTab.DataIdentificator[0]);
			break;
		}
		_lastContentViewController.Show(identificator, isMultiTabbed);
		_currentTab = (tabIdentificator, tabType, isMultiTabbed);
	}

	public void TryRemoveEmptyTabs()
	{
		List<CompositeIdentificator> list = new List<CompositeIdentificator>();
		foreach (CompositeIdentificator id in _currentMinievents.Values)
		{
			IEnumerable<MiniEventActivityTab> enumerable = _tabsManager.Collection.Where((MiniEventActivityTab t) => t.EventIdentificator == id);
			bool flag = false;
			foreach (MiniEventActivityTab item in enumerable)
			{
				CompositeIdentificator identificator = item.DataIdentificator;
				if (item.TabType == TabType.Rating)
				{
					identificator = new CompositeIdentificator(item.CalendarId, item.EventIdentificator[0], item.DataIdentificator[0]);
				}
				flag = _contentables[item.TabType].HasAnyContentAvailable(identificator);
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				list.Add(id);
			}
		}
		foreach (CompositeIdentificator value in _currentMinievents.Values)
		{
			if (!list.Contains(value))
			{
				_miniEventController.Hide(value);
				_miniEventController.Show(value);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			_miniEventController.Hide(list[i]);
		}
		if (list.Any((CompositeIdentificator id) => id[0] == CurrentMiniEvent.EventId))
		{
			InteractFirstMiniEvent();
		}
	}

	public void HandleEmptyActivityTab()
	{
		if (!_miniEventActivityTabsViewController.TryChangeActiveTab(out _currentTab.Item1, out _currentTab.Item2, _currentTab.Item1, _currentTab.Item2))
		{
			TryRemoveEmptyTabs();
		}
	}

	public Transform GetMiniEventViewTransformByCurrencyIdentificator(CompositeIdentificator currencyIdentificator)
	{
		return _miniEventController.GetMiniEventViewTransform(currencyIdentificator);
	}

	public void HandleCurrentTabContent()
	{
		(CompositeIdentificator, TabType, bool) currentTab = _currentTab;
		if ((currentTab.Item1 == default(CompositeIdentificator) && currentTab.Item2 == TabType.None && !currentTab.Item3) || _currentTab.Item1 == default(CompositeIdentificator) || _currentTab.Item1.Identificators == null || !_currentTab.Item1.Identificators.Any())
		{
			return;
		}
		if (_currentTab.Item3)
		{
			if (_currentTab.Item1.Identificators != null && _currentTab.Item1.Identificators.Any())
			{
				HandleTabContent(_currentTab.Item1, _currentTab.Item2, _currentTab.Item3);
			}
		}
		else
		{
			InteractMiniEvent(_currentOpenedMinievent, isForced: true);
			HandleActivityTabs(_currentOpenedMinievent, _currentTab.Item3);
		}
	}

	private void HandleFirstActivityTab(CompositeIdentificator eventIdentificator, bool isMultiTabbed)
	{
		MiniEventActivityTab miniEventActivityTab = (from t in _tabsManager.Collection
			where t.EventIdentificator == eventIdentificator && t.IsAnyContentAvailable.Value
			orderby t.PriorityId
			select t).FirstOrDefault();
		if (miniEventActivityTab != null)
		{
			HandleTabContent(miniEventActivityTab.Identificator, miniEventActivityTab.TabType, isMultiTabbed);
		}
	}

	private void CalculateAnyActiveMiniEvent()
	{
		IsAnyActiveMiniEventReactive.Value = _miniEventController.IsAnyActive();
	}

	private void OnMiniEventStart(MiniEvent minievent)
	{
		_miniEventController.Show(minievent.Identificator);
		if (!_currentMinievents.ContainsKey(minievent.CalendarId))
		{
			_currentMinievents.Add(minievent.CalendarId, minievent.Identificator);
		}
	}

	private void OnMiniEventEnd(MiniEvent minievent)
	{
		_miniEventController.Hide(minievent.Identificator, minievent.CalendarId);
		if (_currentMinievents.ContainsKey(minievent.CalendarId))
		{
			_currentMinievents.Remove(minievent.CalendarId);
		}
		CalculateAnyActiveMiniEvent();
		if (_miniEventsStateService.IsAnyInProgress())
		{
			InteractFirstMiniEvent();
		}
	}
}

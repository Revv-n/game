using System;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events.Content;
using GreenT.Types;
using GreenT.UI;
using UniRx;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventTutorialSystem : IInitializable, IDisposable
{
	private TutorialWindow _eventTutorialWindow;

	private Event _eventValue;

	private IDisposable _isGameActiveStream;

	private readonly IWindowsManager _windowsManager;

	private readonly CalendarQueue _calendarQueue;

	private readonly WindowGroupID _openEventCore;

	private readonly GameStarter _gameStarter;

	private readonly ContentSelectorGroup _contentGroupSelector;

	private readonly EventSettingsProvider _eventSettingsProvider;

	public EventTutorialSystem(IWindowsManager windowsManager, CalendarQueue calendarQueue, WindowGroupID openEventCore, GameStarter gameStarter, ContentSelectorGroup contentGroupSelector, EventSettingsProvider eventSettingsProvider)
	{
		_contentGroupSelector = contentGroupSelector;
		_windowsManager = windowsManager;
		_calendarQueue = calendarQueue;
		_openEventCore = openEventCore;
		_gameStarter = gameStarter;
		_eventSettingsProvider = eventSettingsProvider;
	}

	public void Initialize()
	{
		_isGameActiveStream?.Dispose();
		_isGameActiveStream = ObservableExtensions.Subscribe<bool>(Observable.Where<bool>((IObservable<bool>)_gameStarter.IsGameActive, (Func<bool, bool>)((bool active) => active)), (Action<bool>)delegate
		{
			Subscribe();
		});
	}

	private void Subscribe()
	{
		WindowID windowsID = _openEventCore.GetWindows().Last();
		IWindow window = _windowsManager.GetWindow(windowsID);
		window.OnChangeState -= OnOpen;
		window.OnChangeState += OnOpen;
	}

	private void OnOpen(object sender, EventArgs args)
	{
		if (args is WindowArgs { Active: not false } && _contentGroupSelector.Current == ContentType.Event)
		{
			CalendarModel activeCalendar = _calendarQueue.GetActiveCalendar(EventStructureType.Event);
			Event @event = _eventSettingsProvider.GetEvent(activeCalendar.BalanceId);
			if (@event != null && !@event.IsTutorialEnd)
			{
				_eventTutorialWindow = _windowsManager.Get<EventTutorialWindow>();
				_eventTutorialWindow.Open();
				@event.IsLaunched = true;
				@event.IsTutorialEnd = true;
			}
		}
	}

	public void Dispose()
	{
		_isGameActiveStream?.Dispose();
	}
}

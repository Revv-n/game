using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.UI;
using Merge.Meta.RoomObjects;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventButtonSubscription : MonoView<CalendarModel>
{
	[SerializeField]
	private Button button;

	[SerializeField]
	private WindowID comingSoonWindow;

	[SerializeField]
	private WindowOpener openComingSoon;

	[SerializeField]
	private WindowOpener openStartWindow;

	[SerializeField]
	private WindowOpener openProgressWindow;

	[SerializeField]
	private Button eventStartButton;

	private ComingSoonWindow _comingSoonWindow;

	private EventSettingsProvider _eventSettingsProvider;

	private IWindowsManager _windowsManager;

	private EventProgressView _eventProgressView;

	private Event _event;

	private IDisposable _eventClickStream;

	private IDisposable _openWindowStream;

	[Inject]
	private void InnerInit(IWindowsManager windowsManager, EventProgressView eventProgressView, EventSettingsProvider eventSettingsProvider)
	{
		_windowsManager = windowsManager;
		_eventProgressView = eventProgressView;
		_eventSettingsProvider = eventSettingsProvider;
	}

	public override void Set(CalendarModel source)
	{
		base.Set(source);
		if (_eventSettingsProvider.TryGetEvent(base.Source.BalanceId, out var @event))
		{
			if (_event != null)
			{
				_eventClickStream?.Dispose();
			}
			_event = @event;
			_eventClickStream = eventStartButton.OnClickAsObservable().Subscribe(delegate
			{
				_event.SetFirstTimeStarted();
			});
		}
	}

	private void Awake()
	{
		_openWindowStream = button.OnClickAsObservable().Subscribe(delegate
		{
			OpenWindow();
		});
	}

	private void OnDestroy()
	{
		_openWindowStream?.Dispose();
		_eventClickStream?.Dispose();
	}

	private void OpenWindow()
	{
		if (_comingSoonWindow == null)
		{
			_comingSoonWindow = _windowsManager.GetWindow(comingSoonWindow) as ComingSoonWindow;
		}
		switch (base.Source.CalendarState.Value)
		{
		case EntityStatus.Blocked:
			_comingSoonWindow.Set(base.Source);
			openComingSoon.Click();
			break;
		case EntityStatus.InProgress:
			_eventProgressView.SetViewState(1);
			openStartWindow.Click();
			break;
		case EntityStatus.Complete:
			_eventProgressView.SetViewState(2);
			openProgressWindow.Click();
			break;
		default:
			throw new Exception().SendException($"{GetType().Name}: no behaviour for {base.Source.CalendarState.Value}");
		}
	}
}

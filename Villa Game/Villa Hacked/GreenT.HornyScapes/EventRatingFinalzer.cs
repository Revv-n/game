using System;
using GreenT.HornyScapes.Events;
using GreenT.HornyScapes.Lootboxes;
using GreenT.UI;
using StripClub.UI.Rewards;
using UniRx;

namespace GreenT.HornyScapes;

public sealed class EventRatingFinalzer : IDisposable
{
	private readonly IWindowsManager _windowsManager;

	private IDisposable _subscribeDisposable;

	private RewardsWindow _rewardsWindow;

	private EventRatingWindowView _eventRatingWindowView;

	private Event _targetEvent;

	public EventRatingFinalzer(IWindowsManager windowsManager)
	{
		_windowsManager = windowsManager;
	}

	public void Init(Event targetEvent)
	{
		_targetEvent = targetEvent;
		if (_eventRatingWindowView == null)
		{
			_eventRatingWindowView = _windowsManager.Get<EventRatingWindowView>();
		}
		_eventRatingWindowView.TryActivateElements(_targetEvent.GlobalRatingId, _targetEvent.GroupRatingId);
		_eventRatingWindowView.InitDescription(_targetEvent.EventId);
		_eventRatingWindowView.InitGlobal(_targetEvent.EventId, _targetEvent.CalendarId, _targetEvent.GlobalRatingId);
		_eventRatingWindowView.InitGroup(_targetEvent.EventId, _targetEvent.CalendarId, _targetEvent.GroupRatingId);
		_eventRatingWindowView.InitButtons(_targetEvent.GlobalRatingId);
		_eventRatingWindowView.InitBackground(_targetEvent.Bundle.Type);
	}

	public void Dispose()
	{
		_subscribeDisposable?.Dispose();
	}

	public void SubscribeRewardsWindowClose(int skipTimes)
	{
		if (_rewardsWindow == null)
		{
			_rewardsWindow = _windowsManager.Get<RewardsWindow>();
		}
		Dispose();
		_subscribeDisposable = ObservableExtensions.Subscribe<Lootbox>(Observable.Take<Lootbox>(Observable.Skip<Lootbox>(_rewardsWindow.OnCloseWithLootbox, skipTimes), 1), (Action<Lootbox>)delegate
		{
			_eventRatingWindowView.Open();
		});
	}
}

using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Events;

public class EventTutorialStrategyResolver : MonoView
{
	[SerializeField]
	private GameObject _eventContainer;

	[SerializeField]
	private GameObject _ratingContainer;

	[SerializeField]
	private GameObject _battlePassContainer;

	private CalendarQueue _calendarQueue;

	private ITutorialStateStrategy _currentStrategy;

	private EventTutorialStateStrategy _eventStrategy;

	private RatingTutorialStateStrategy _ratingStrategy;

	private BattlePassTutorialStateStrategy _battlePassStrategy;

	[Inject]
	public void Construct(CalendarQueue calendarQueue)
	{
		_calendarQueue = calendarQueue;
	}

	public void Show()
	{
		HideContainer();
		CheckStrategy();
		ShowContainer();
	}

	private void Awake()
	{
		_eventStrategy = new EventTutorialStateStrategy(_eventContainer);
		_ratingStrategy = new RatingTutorialStateStrategy(_ratingContainer);
		_battlePassStrategy = new BattlePassTutorialStateStrategy(_battlePassContainer);
		_currentStrategy = _eventStrategy;
	}

	private void ShowContainer()
	{
		_currentStrategy.SetActive(isActive: true);
	}

	private void HideContainer()
	{
		_currentStrategy.SetActive(isActive: false);
	}

	private void CheckStrategy()
	{
		EventMapper eventMapper = _calendarQueue.GetActiveCalendar(EventStructureType.Event).EventMapper as EventMapper;
		ITutorialStateStrategy currentStrategy;
		if (eventMapper.rating_id == 0 && eventMapper.group_rating_id == 0)
		{
			if (eventMapper.bp_id == 0)
			{
				ITutorialStateStrategy eventStrategy = _eventStrategy;
				currentStrategy = eventStrategy;
			}
			else
			{
				ITutorialStateStrategy eventStrategy = _battlePassStrategy;
				currentStrategy = eventStrategy;
			}
		}
		else
		{
			ITutorialStateStrategy eventStrategy = _ratingStrategy;
			currentStrategy = eventStrategy;
		}
		_currentStrategy = currentStrategy;
	}
}

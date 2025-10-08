using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using StripClub.UI;
using UnityEngine;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventViewController : BaseViewController<MiniEvent, MiniEventView>
{
	private readonly CalendarQueue _calendarQueue;

	public MiniEventViewController(CalendarQueue calendarQueue, IManager<MiniEvent> settingsProvider, IViewManager<MiniEvent, MiniEventView> viewManager)
		: base(settingsProvider, viewManager)
	{
		_calendarQueue = calendarQueue;
	}

	public bool IsAnyActive()
	{
		return _viewManager.VisibleViews.Any();
	}

	public bool IsActive(CompositeIdentificator eventIdentificator)
	{
		return _viewManager.VisibleViews.Any((MiniEventView v) => v.Source.Identificator == eventIdentificator);
	}

	public Transform GetMiniEventViewTransform(CompositeIdentificator currencyIdentificator)
	{
		return _viewManager.VisibleViews.FirstOrDefault((MiniEventView view) => view.Source.CurrencyIdentificator == currencyIdentificator).transform;
	}

	public override void Show(CompositeIdentificator identificator, bool isMultiTabbed = false)
	{
		base.Show(identificator, isMultiTabbed);
		OrderViews();
	}

	public MiniEvent ShowFirst()
	{
		MiniEventView miniEventView = GetOrderedViews().FirstOrDefault();
		if (miniEventView != null)
		{
			miniEventView.ForceSelfInteract();
			return miniEventView.Source;
		}
		return null;
	}

	public void Hide(CompositeIdentificator identificator, int calendarId = 0)
	{
		MiniEventView view = GetView(identificator, calendarId);
		if (view != null)
		{
			view.Display(display: false);
		}
	}

	public void HandlePress(CompositeIdentificator identificator)
	{
		foreach (MiniEventView visibleView in _viewManager.VisibleViews)
		{
			visibleView.SetState(identificator);
		}
	}

	public MiniEventView GetView(CompositeIdentificator identificator, int calendarId = 0)
	{
		if (calendarId != 0)
		{
			return _viewManager.VisibleViews.FirstOrDefault((MiniEventView v) => v.Identificator == identificator && v.Source.CalendarId == calendarId);
		}
		return _viewManager.VisibleViews.FirstOrDefault((MiniEventView v) => v.Identificator == identificator);
	}

	protected override IEnumerable<MiniEvent> GetSources(CompositeIdentificator eventIdentificator)
	{
		IEnumerable<CalendarModel> activeMinieventCalendars = _calendarQueue.GetAllActiveInProgressCalendars(EventStructureType.Mini);
		return _manager.Collection.Where((MiniEvent minievent) => minievent.Identificator == eventIdentificator && activeMinieventCalendars.Any((CalendarModel calendar) => calendar.UniqID == minievent.CalendarId));
	}

	protected override void OrderViews()
	{
		foreach (MiniEventView orderedView in GetOrderedViews())
		{
			orderedView.transform.SetAsLastSibling();
		}
	}

	protected override IEnumerable<MiniEventView> GetOrderedViews()
	{
		return _viewManager.VisibleViews.OrderBy((MiniEventView view) => view.Source.PriorityId);
	}
}

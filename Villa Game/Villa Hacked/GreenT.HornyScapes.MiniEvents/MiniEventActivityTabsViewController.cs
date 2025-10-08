using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Events;
using GreenT.Types;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventActivityTabsViewController : BaseViewController<MiniEventActivityTab, MiniEventActivityTabView>
{
	private readonly CalendarQueue _calendarQueue;

	public MiniEventActivityTabsViewController(CalendarQueue calendarQueue, IManager<MiniEventActivityTab> manager, IViewManager<MiniEventActivityTab, MiniEventActivityTabView> viewManager)
		: base(manager, viewManager)
	{
		_calendarQueue = calendarQueue;
	}

	public override void Show(CompositeIdentificator identificator, bool isMultiTabbed = false)
	{
		base.Show(identificator, isMultiTabbed);
		OrderViews();
	}

	public void HandlePress(CompositeIdentificator identificator, TabType tabType)
	{
		foreach (MiniEventActivityTabView visibleView in _viewManager.VisibleViews)
		{
			visibleView.SetState(identificator, tabType);
		}
	}

	public bool TryChangeActiveTab(out CompositeIdentificator identificator, out TabType tabType, CompositeIdentificator currentIdentificator, TabType currentTabType)
	{
		MiniEventActivityTabView miniEventActivityTabView = _viewManager.VisibleViews.FirstOrDefault((MiniEventActivityTabView view) => view.Source.Identificator == currentIdentificator && view.Source.TabType == currentTabType);
		if (miniEventActivityTabView != null)
		{
			miniEventActivityTabView.Display(display: false);
		}
		MiniEventActivityTabView miniEventActivityTabView2 = _viewManager.VisibleViews.FirstOrDefault();
		if (miniEventActivityTabView2 != null)
		{
			miniEventActivityTabView2.ForceSelfInteract();
			identificator = miniEventActivityTabView2.Source.Identificator;
			tabType = miniEventActivityTabView2.Source.TabType;
			return true;
		}
		identificator = default(CompositeIdentificator);
		tabType = TabType.None;
		return false;
	}

	protected override IEnumerable<MiniEventActivityTab> GetSources(CompositeIdentificator identificator)
	{
		IEnumerable<CalendarModel> activeMinieventCalendars = _calendarQueue.GetAllActiveInProgressCalendars(EventStructureType.Mini);
		return _manager.Collection.Where((MiniEventActivityTab t) => t.EventIdentificator == identificator && t.IsAnyContentAvailable.Value && activeMinieventCalendars.Any((CalendarModel calendar) => calendar.UniqID == t.CalendarId));
	}

	protected override void OrderViews()
	{
		foreach (MiniEventActivityTabView orderedView in GetOrderedViews())
		{
			orderedView.transform.SetAsLastSibling();
		}
	}

	protected override IEnumerable<MiniEventActivityTabView> GetOrderedViews()
	{
		return _viewManager.VisibleViews.OrderBy((MiniEventActivityTabView view) => view.Source.PriorityId);
	}
}

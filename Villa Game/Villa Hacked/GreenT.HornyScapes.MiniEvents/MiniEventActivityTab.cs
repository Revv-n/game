using GreenT.Types;
using UniRx;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventActivityTab : IIdentifiable
{
	public int PriorityId { get; }

	public int CalendarId { get; }

	public string BackgroundBundleKey { get; private set; }

	public string IconBundleKey { get; private set; }

	public ReactiveProperty<bool> IsAnyActionAvailable { get; set; }

	public ReactiveProperty<bool> IsAnyContentAvailable { get; set; }

	public CompositeIdentificator Identificator { get; }

	public CompositeIdentificator DataIdentificator { get; }

	public CompositeIdentificator EventIdentificator { get; }

	public TabType TabType { get; }

	public MiniEventActivityTab(int calendarId, int priorityId, string iconBundleKey, string backgroundBundleKey, CompositeIdentificator eventIdentificator, CompositeIdentificator tabIdentificator, CompositeIdentificator dataIdentificator, TabType tabType)
	{
		PriorityId = priorityId;
		CalendarId = calendarId;
		IconBundleKey = iconBundleKey;
		BackgroundBundleKey = backgroundBundleKey;
		EventIdentificator = eventIdentificator;
		Identificator = tabIdentificator;
		DataIdentificator = dataIdentificator;
		TabType = tabType;
		IsAnyActionAvailable = new ReactiveProperty<bool>();
		IsAnyContentAvailable = new ReactiveProperty<bool>();
	}
}

using System.Collections.Generic;
using GreenT.Types;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventRatingConfigHandler : BaseMiniEventConfigHandler
{
	private const int PRIORITY = 1;

	private const bool IS_GLOBAL = true;

	private readonly RatingDataFactory _ratingDataFactory;

	private readonly MiniEventRatingControllerFactory _ratingControllerFactory;

	public MiniEventRatingConfigHandler(ConfigType configType, MiniEventTabsManager miniEventTabsManager, MiniEventActivityTabAdministrator miniEventActivityTabAdministrator, RatingDataFactory ratingDataFactory, MiniEventRatingControllerFactory ratingControllerFactory)
		: base(configType, miniEventTabsManager, miniEventActivityTabAdministrator)
	{
		_ratingDataFactory = ratingDataFactory;
		_ratingControllerFactory = ratingControllerFactory;
	}

	public override IEnumerable<IController> Handle<TMapper>(TMapper ratingMapper, int eventId, int activityId, int calendarId)
	{
		GreenT.HornyScapes.Rating rating = ratingMapper as GreenT.HornyScapes.Rating;
		List<IController> list = new List<IController>();
		CompositeIdentificator eventIdentificator = new CompositeIdentificator(eventId, activityId);
		CompositeIdentificator tabIdentificator = new CompositeIdentificator(activityId);
		CompositeIdentificator dataIdentificator = new CompositeIdentificator(activityId);
		MiniEventActivityTab miniEventActivityTab = CreateActivityTab(calendarId, 1, GetIconBundleKey(activityId, TabType.Rating), GetBackgroundBundleKey(activityId, TabType.Rating), eventIdentificator, tabIdentificator, dataIdentificator, TabType.Rating);
		AdministrateActivityTab(miniEventActivityTab);
		AddMiniEventActivityTab(miniEventActivityTab);
		RatingData ratingData = _ratingDataFactory.Create(eventId, calendarId, isGlobal: true, rating);
		RatingController item = _ratingControllerFactory.Create(ratingData, isCurrencyTrackable: true);
		list.Add(item);
		return list;
	}
}

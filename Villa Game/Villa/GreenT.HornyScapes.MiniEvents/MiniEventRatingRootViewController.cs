using System.Collections.Generic;
using System.Linq;
using GreenT.Types;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventRatingRootViewController : BaseViewController<RatingData, RatingView>
{
	public MiniEventRatingRootViewController(IManager<RatingData> manager, IViewManager<RatingData, RatingView> viewManager)
		: base(manager, viewManager)
	{
	}

	protected override IEnumerable<RatingData> GetSources(CompositeIdentificator identificator)
	{
		return _manager.Collection.Where((RatingData ratingData) => ratingData.CalendarID == identificator[0] && ratingData.EventID == identificator[1] && ratingData.TargetRating.ID == identificator[2]);
	}
}

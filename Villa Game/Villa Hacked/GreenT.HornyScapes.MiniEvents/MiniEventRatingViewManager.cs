using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventRatingViewManager : ViewManager<RatingData, RatingView>
{
	protected override bool CheckAvailableView(RatingView view, RatingData source)
	{
		if (view.Source != null)
		{
			if (base.CheckAvailableView(view, source) && view.Source.EventID == source.EventID && view.Source.CalendarID == source.CalendarID)
			{
				return view.Source.TargetRating.ID == source.TargetRating.ID;
			}
			return false;
		}
		return base.CheckAvailableView(view, source);
	}
}

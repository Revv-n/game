using GreenT.HornyScapes.Saves;
using StripClub.UI;

namespace GreenT.HornyScapes.MiniEvents;

public sealed class MiniEventRatingControllerFactory : RatingControllerFactory
{
	private readonly MiniEventTimerController _miniEventTimerController;

	public MiniEventRatingControllerFactory(WalletProvider walletProvider, RatingService ratingService, MiniEventTimerController miniEventTimerController, RatingControllerManager ratingControllerManager, SaveController saveController, RatingRewardService ratingRewardService)
		: base(walletProvider, ratingService, ratingControllerManager, saveController, ratingRewardService)
	{
		_miniEventTimerController = miniEventTimerController;
	}

	public override RatingController Create(RatingData ratingData, bool isCurrencyTrackable = true)
	{
		GenericTimer timer = _miniEventTimerController.TryGetTimerByID(ratingData.EventID, ratingData.CalendarID);
		RatingController ratingController = base.Create(ratingData, isCurrencyTrackable);
		ratingController.Timer = timer;
		return ratingController;
	}
}

using System;
using GreenT.HornyScapes.BattlePassSpace;
using GreenT.HornyScapes.Saves;
using StripClub.UI;

namespace GreenT.HornyScapes;

public sealed class EventRatingControllerFactory : RatingControllerFactory
{
	public EventRatingControllerFactory(WalletProvider walletProvider, RatingService ratingService, RatingControllerManager ratingControllerManager, SaveController saveController, RatingRewardService ratingRewardService)
		: base(walletProvider, ratingService, ratingControllerManager, saveController, ratingRewardService)
	{
	}

	public RatingController Create(RatingData ratingData, bool isCurrencyTrackable, CalendarModel calendarModel)
	{
		GenericTimer timer = new GenericTimer(TimeSpan.FromSeconds(calendarModel.RemainingTime));
		RatingController ratingController = Create(ratingData, isCurrencyTrackable);
		ratingController.Timer = timer;
		return ratingController;
	}
}

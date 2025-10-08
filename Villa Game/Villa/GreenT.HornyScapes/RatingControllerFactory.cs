using GreenT.HornyScapes.Saves;
using Zenject;

namespace GreenT.HornyScapes;

public abstract class RatingControllerFactory : IFactory<RatingData, bool, RatingController>, IFactory
{
	protected readonly WalletProvider _walletProvider;

	protected readonly RatingService _ratingService;

	protected readonly RatingControllerManager _ratingControllerManager;

	private readonly SaveController _saveController;

	private readonly RatingRewardService _ratingRewardService;

	public RatingControllerFactory(WalletProvider walletProvider, RatingService ratingService, RatingControllerManager ratingControllerManager, SaveController saveController, RatingRewardService ratingRewardService)
	{
		_walletProvider = walletProvider;
		_ratingService = ratingService;
		_ratingControllerManager = ratingControllerManager;
		_saveController = saveController;
		_ratingRewardService = ratingRewardService;
	}

	public virtual RatingController Create(RatingData ratingData, bool isCurrencyTrackable)
	{
		_walletProvider.TryGetContainer(ratingData.TargetRating.CurrencyType, out var container, ratingData.TargetRating.CurrencyIdentificator);
		RatingController ratingController = new RatingController(container, _ratingService, ratingData, _saveController, _ratingRewardService, isCurrencyTrackable);
		_ratingControllerManager.Add(ratingController);
		return ratingController;
	}
}

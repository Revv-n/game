using GreenT.Types;
using StripClub.Model;
using Zenject;

namespace GreenT.HornyScapes;

public sealed class RatingFactory : IFactory<RatingMapper, Rating>, IFactory
{
	private readonly MatchmakingManager _matchmakingManager;

	public RatingFactory(MatchmakingManager matchmakingManager)
	{
		_matchmakingManager = matchmakingManager;
	}

	public Rating Create(RatingMapper mapper)
	{
		Matchmaking matchmakingInfo = _matchmakingManager.GetMatchmakingInfo(mapper.matchmaking);
		(CurrencyType, CompositeIdentificator) tuple = PriceResourceHandler.ParsePriceSourse(mapper.currency);
		return new Rating(mapper.id, mapper.range, tuple.Item1, tuple.Item2, matchmakingInfo);
	}
}

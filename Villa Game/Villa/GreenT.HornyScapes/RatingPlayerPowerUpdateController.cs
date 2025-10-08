using System.Linq;
using GreenT.Data;

namespace GreenT.HornyScapes;

public sealed class RatingPlayerPowerUpdateController
{
	private readonly TournamentPointsManager _tournamentPointsManager;

	private readonly TournamentPointsStorage _tournamentPointsStorage;

	public RatingPlayerPowerUpdateController(TournamentPointsManager tournamentPointsManager, TournamentPointsStorage tournamentPointsStorage, ISaver saver)
	{
		_tournamentPointsManager = tournamentPointsManager;
		_tournamentPointsStorage = tournamentPointsStorage;
		saver.Add(_tournamentPointsStorage);
	}

	public void CalculateAdditivePoints(int place)
	{
		TournamentPointsMapper tournamentPointsMapper = _tournamentPointsManager.Collection.FirstOrDefault((TournamentPointsMapper tournamentPoints) => place >= tournamentPoints.lower_border && place <= tournamentPoints.upper_border);
		if (tournamentPointsMapper != null)
		{
			_tournamentPointsStorage.AdditivePoints += tournamentPointsMapper.tournament_points;
		}
	}
}

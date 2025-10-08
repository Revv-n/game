using System.Linq;
using GreenT.Model.Collections;

namespace GreenT.HornyScapes;

public sealed class MatchmakingManager : SimpleManager<Matchmaking>
{
	public Matchmaking GetMatchmakingInfo(int id)
	{
		return collection.FirstOrDefault((Matchmaking _matchmaking) => _matchmaking.ID == id);
	}
}

using System.Collections.Generic;

namespace StripClub.Model;

public interface IPlayerPortfolio
{
	Position FindPosition(int id);

	void AddPosition(Position position);

	IEnumerable<Position> GetPositions();
}

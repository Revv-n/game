using System.Collections.Generic;

namespace GreenT.HornyScapes.Saves;

public class SaveEventClusterManager : Dictionary<SaveMode, SaveEventManager>
{
	private SaveMode current;

	public void ChangeMode(SaveMode nextMode)
	{
		if (current != nextMode)
		{
			base[current].StopTrack();
			current = nextMode;
			base[current].Track();
		}
	}
}

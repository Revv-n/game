using System;
using System.Collections.Generic;
using StripClub.Model;

namespace StripClub.Utility;

public static class TimeskipCalculator
{
	public static int RestoreEnergyAmount(IPlayerBasics playerBasics, TimeSpan timeskipDuration)
	{
		return playerBasics.Energy.GetRestoreAmount(timeskipDuration);
	}

	public static IEnumerable<int> OpeningDialogueIDs(TimeSpan timeskipDuration)
	{
		throw new NotImplementedException();
	}
}

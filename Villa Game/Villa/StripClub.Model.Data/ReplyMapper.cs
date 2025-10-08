using System;
using StripClub.Extensions;

namespace StripClub.Model.Data;

[Serializable]
public class ReplyMapper
{
	public int energy;

	public int maxEnergy;

	public DateTime lastRestore;

	public TimeSpan tickFrequency;

	public ReplyMapper(RestorableValue<int> Energy)
	{
		energy = Energy.Value;
		maxEnergy = Energy.Max;
		lastRestore = Energy.LastRestoreDate;
		tickFrequency = Energy.RestorePeriod;
	}
}

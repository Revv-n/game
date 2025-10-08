using GreenT.Bonus;
using JetBrains.Annotations;

namespace GreenT.HornyScapes.Booster.Effect;

public class BoosterBonusParameters : BonusParameters
{
	[CanBeNull]
	public string SummonType { get; }

	[CanBeNull]
	public int[] SummonTabID { get; }

	public BoosterBonusParameters(int uniqParentID, string name, BonusType bonusType, int value, [CanBeNull] string summonType, [CanBeNull] int[] summonTabID)
		: base(uniqParentID, name, bonusType, value)
	{
		SummonType = summonType;
		SummonTabID = summonTabID;
	}
}

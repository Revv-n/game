using GreenT.Bonus;

namespace GreenT.HornyScapes.Card.Bonus;

public class CharacterBonusParameters : BonusParameters
{
	public bool AffectAll { get; }

	public int[] AffectedSpawnerID { get; }

	public CharacterBonusParameters(int uniqParentID, string name, BonusType bonusType, double[] value, bool affectAll, int[] affectedSpawnerID)
		: base(uniqParentID, name, bonusType, value)
	{
		AffectAll = affectAll;
		AffectedSpawnerID = affectedSpawnerID;
	}
}

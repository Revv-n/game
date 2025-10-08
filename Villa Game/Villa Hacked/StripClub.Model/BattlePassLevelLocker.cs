namespace StripClub.Model;

public class BattlePassLevelLocker : EqualityLocker<int>
{
	public BattlePassLevelLocker(int companyLevel, Restriction restrictor)
		: base(companyLevel, restrictor)
	{
	}
}

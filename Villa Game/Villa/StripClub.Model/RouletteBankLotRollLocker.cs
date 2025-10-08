namespace StripClub.Model;

public sealed class RouletteBankLotRollLocker : RouletteLotRollLocker
{
	public RouletteBankLotRollLocker(int targetId, int rollsRequired, Restriction condition)
		: base(targetId, rollsRequired, condition)
	{
	}
}

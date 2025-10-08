using GreenT.HornyScapes;

namespace StripClub.Model;

public class RouletteLotRollLocker : Locker
{
	private readonly int _rollsRequired;

	private readonly Restriction _condition;

	public int TargetId { get; }

	public RouletteLotRollLocker(int targetId, int rollsRequired, Restriction condition)
	{
		TargetId = targetId;
		_rollsRequired = rollsRequired;
		_condition = condition;
	}

	public override void Initialize()
	{
		switch (_condition)
		{
		case Restriction.Max:
			isOpen.Value = false;
			break;
		case Restriction.Min:
			isOpen.Value = true;
			break;
		}
	}

	public void Set(RouletteLot rouletteLot)
	{
		if (rouletteLot.ID == TargetId)
		{
			switch (_condition)
			{
			case Restriction.Max:
				isOpen.Value = rouletteLot.OverallRollCount >= _rollsRequired;
				break;
			case Restriction.Min:
				isOpen.Value = rouletteLot.OverallRollCount < _rollsRequired;
				break;
			}
		}
	}
}

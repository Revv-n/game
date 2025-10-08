namespace GreenT.HornyScapes.MergeStore;

public class SpendHardForRechargeSignal : IValuableSignal
{
	public int Value { get; }

	public SpendHardForRechargeSignal(int value)
	{
		Value = value;
	}
}

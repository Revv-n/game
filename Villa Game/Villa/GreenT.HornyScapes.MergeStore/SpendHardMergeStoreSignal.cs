namespace GreenT.HornyScapes.MergeStore;

public class SpendHardMergeStoreSignal : IValuableSignal
{
	public int Value { get; }

	public SpendHardMergeStoreSignal(int value)
	{
		Value = value;
	}
}

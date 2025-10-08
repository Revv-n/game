using GreenT.HornyScapes.MergeStore;

namespace GreenT.HornyScapes.MergeCore;

public class SpendHardBubbleSignal : IValuableSignal
{
	public int Value { get; }

	public SpendHardBubbleSignal(int value)
	{
		Value = value;
	}
}

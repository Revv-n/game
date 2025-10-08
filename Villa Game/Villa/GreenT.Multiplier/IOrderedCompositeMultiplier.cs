namespace GreenT.Multiplier;

public interface IOrderedCompositeMultiplier : IMultiplier
{
	void Add(IMultiplier multiplier, int order);

	void Remove(IMultiplier multiplier);

	bool Contains(IMultiplier multiplier);
}

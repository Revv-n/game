namespace GreenT.Multiplier;

public interface ICompositeMultiplier : IMultiplier
{
	void Add(IMultiplier multiplier);

	void Remove(IMultiplier multiplier);

	bool Contains(IMultiplier multiplier);
}

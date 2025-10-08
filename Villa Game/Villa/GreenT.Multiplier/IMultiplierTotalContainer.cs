namespace GreenT.Multiplier;

public interface IMultiplierTotalContainer<TKey> : IMultiplierContainer<TKey>
{
	void AddToTotal(IMultiplier multiplier);

	void RemoveFromTotal(IMultiplier multiplier);
}

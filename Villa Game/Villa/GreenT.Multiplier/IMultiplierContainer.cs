using System.Collections.Generic;

namespace GreenT.Multiplier;

public interface IMultiplierContainer<TKey>
{
	void Add(TKey key, IMultiplier multiplier);

	void Remove(TKey key, IMultiplier multiplier);

	IMultiplier GetMultiplier(TKey key);

	IEnumerable<TKey> Keys();
}

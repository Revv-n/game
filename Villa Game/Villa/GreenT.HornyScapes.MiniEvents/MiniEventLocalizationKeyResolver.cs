using System.Collections.Generic;

namespace GreenT.HornyScapes.MiniEvents;

public class MiniEventLocalizationKeyResolver<TKey>
{
	protected Dictionary<TKey, string> _localizationKeys;

	public virtual string GetKey(TKey key)
	{
		_localizationKeys.TryGetValue(key, out var value);
		return value;
	}
}

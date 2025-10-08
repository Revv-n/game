using System.Collections.Generic;
using GreenT.HornyScapes.Events;

namespace GreenT.HornyScapes;

public abstract class BaseLastChanceBundleProvider<T> where T : IBundleData
{
	private Dictionary<int, T> _bundles;

	public BaseLastChanceBundleProvider()
	{
		_bundles = new Dictionary<int, T>();
	}

	public void TryAdd(int id, T bundleData)
	{
		if (!Contains(id))
		{
			_bundles.Add(id, bundleData);
		}
	}

	public T TryGet(int id)
	{
		if (!Contains(id))
		{
			return default(T);
		}
		return _bundles[id];
	}

	public bool Contains(int id)
	{
		return _bundles.ContainsKey(id);
	}

	public void Remove(int id)
	{
		_bundles.Remove(id);
	}
}

using System;
using System.Collections.Generic;

namespace GreenT.HornyScapes;

public abstract class LightWeightFactory<T>
{
	private Dictionary<Type, T> _lightWeights;

	protected LightWeightFactory()
	{
		_lightWeights = new Dictionary<Type, T>();
	}

	public T GetLightweightObject<O>() where O : class
	{
		Type typeFromHandle = typeof(O);
		if (!_lightWeights.ContainsKey(typeFromHandle))
		{
			_lightWeights[typeFromHandle] = CreateLightWeight();
		}
		return _lightWeights[typeFromHandle];
	}

	protected abstract T CreateLightWeight();
}

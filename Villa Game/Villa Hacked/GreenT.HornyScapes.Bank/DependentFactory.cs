using System;
using System.Collections.Generic;
using Zenject;

namespace GreenT.HornyScapes.Bank;

public class DependentFactory<TKey, TFactoryValue> : IFactory<TKey, TFactoryValue>, IFactory
{
	private Dictionary<TKey, IFactory<TFactoryValue>> factoryDictionary;

	public IFactory<TFactoryValue> this[TKey source]
	{
		set
		{
			factoryDictionary[source] = value;
		}
	}

	public DependentFactory()
	{
		factoryDictionary = new Dictionary<TKey, IFactory<TFactoryValue>>();
	}

	public void Set(TKey param, IFactory<TFactoryValue> factory)
	{
		factoryDictionary[param] = factory;
	}

	public TFactoryValue Create(TKey param)
	{
		try
		{
			return ((IFactory<_003F>)(object)factoryDictionary[param]).Create();
		}
		catch (Exception innerException)
		{
			throw innerException.SendException("Error on creating " + typeof(TFactoryValue).ToString() + " with parameter: " + param.ToString() + " of type " + typeof(TKey).Name);
		}
	}
}

using System;
using System.Collections.Generic;
using StripClub.Model;
using StripClub.Model.Shop;

namespace GreenT.HornyScapes.Constants;

public class Constants : IConstants<float>, IConstants<int>, IConstants<string>, IConstants<Price<int>>, IConstants<ILocker>
{
	private Dictionary<string, int> intConstants = new Dictionary<string, int>();

	private Dictionary<string, float> floatConstants = new Dictionary<string, float>();

	private Dictionary<string, string> stringConstants = new Dictionary<string, string>();

	private Dictionary<string, Price<int>> priceConstants = new Dictionary<string, Price<int>>();

	private Dictionary<string, ILocker> lockerConstants = new Dictionary<string, ILocker>();

	float IConstants<float>.this[string key] => GetValue(floatConstants, key);

	int IConstants<int>.this[string key] => GetValue(intConstants, key);

	string IConstants<string>.this[string key] => GetValue(stringConstants, key);

	Price<int> IConstants<Price<int>>.this[string key] => GetValue(priceConstants, key);

	public ILocker this[string key] => GetValue(lockerConstants, key);

	public void Add(string key, object value)
	{
		if (!(value is int value2))
		{
			if (!(value is float value3))
			{
				if (!(value is string value4))
				{
					if (!(value is Price<int> value5))
					{
						if (!(value is ILocker value6))
						{
							throw new NotImplementedException("No behaviour for this type of constant:" + value.GetType());
						}
						lockerConstants.Add(key, value6);
					}
					else
					{
						priceConstants.Add(key, value5);
					}
				}
				else
				{
					stringConstants.Add(key, value4);
				}
			}
			else
			{
				floatConstants.Add(key, value3);
			}
		}
		else
		{
			intConstants.Add(key, value2);
		}
	}

	private T GetValue<T>(IDictionary<string, T> dictionary, string key)
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		throw new KeyNotFoundException("Нет константы: \"" + key + "\" типа: " + typeof(T).ToString()).LogException();
	}

	bool IConstants<int>.TryGetValue(string key, out int value)
	{
		return intConstants.TryGetValue(key, out value);
	}

	bool IConstants<float>.TryGetValue(string key, out float value)
	{
		return floatConstants.TryGetValue(key, out value);
	}

	bool IConstants<string>.TryGetValue(string key, out string value)
	{
		return stringConstants.TryGetValue(key, out value);
	}

	bool IConstants<ILocker>.TryGetValue(string key, out ILocker value)
	{
		return lockerConstants.TryGetValue(key, out value);
	}

	bool IConstants<Price<int>>.TryGetValue(string key, out Price<int> value)
	{
		return priceConstants.TryGetValue(key, out value);
	}
}

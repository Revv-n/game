using System;
using System.Globalization;
using System.Text.RegularExpressions;
using StripClub.Model;
using StripClub.Model.Shop;
using Zenject;

namespace GreenT.HornyScapes.Constants;

public class ConstantsDistributor : ICollectionSetter<ConstantMapper>
{
	private readonly Constants constants;

	private readonly IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory;

	public ConstantsDistributor(Constants constants, IFactory<UnlockType, string, LockerSourceType, ILocker> lockerFactory)
	{
		this.constants = constants;
		this.lockerFactory = lockerFactory;
	}

	public void Add(ConstantMapper mapper)
	{
		IFormatProvider provider = new NumberFormatInfo();
		switch (mapper.Type)
		{
		case "int":
		{
			int num = int.Parse(mapper.Value, NumberStyles.Integer, provider);
			constants.Add(mapper.Name, num);
			return;
		}
		case "float":
		{
			float num2 = float.Parse(mapper.Value, NumberStyles.Float, provider);
			constants.Add(mapper.Name, num2);
			return;
		}
		case "string":
			constants.Add(mapper.Name, mapper.Value);
			return;
		}
		float result2;
		ILocker locker;
		Price<int> price;
		Price<float> price2;
		if (int.TryParse(mapper.Value, NumberStyles.Integer, provider, out var result))
		{
			constants.Add(mapper.Name, result);
		}
		else if (float.TryParse(mapper.Value, NumberStyles.Float, provider, out result2))
		{
			constants.Add(mapper.Name, result2);
		}
		else if (TryParseLocker(mapper.Value, out locker))
		{
			constants.Add(mapper.Name, locker);
		}
		else if (Price<int>.TryParse(mapper.Value, out price))
		{
			constants.Add(mapper.Name, price);
		}
		else if (Price<float>.TryParse(mapper.Value, out price2))
		{
			constants.Add(mapper.Name, price2);
		}
		else
		{
			constants.Add(mapper.Name, mapper.Value);
		}
	}

	public void Add(params ConstantMapper[] mappers)
	{
		foreach (ConstantMapper mapper in mappers)
		{
			Add(mapper);
		}
	}

	private bool TryParseLocker(string s, out ILocker locker)
	{
		MatchCollection matchCollection = new Regex("\\w+").Matches(s);
		if (matchCollection.Count != 2 || !Enum.TryParse<UnlockType>(matchCollection[1].Value, out var result))
		{
			locker = null;
			return false;
		}
		locker = lockerFactory.Create(result, matchCollection[0].Value, LockerSourceType.Constant);
		return true;
	}
}

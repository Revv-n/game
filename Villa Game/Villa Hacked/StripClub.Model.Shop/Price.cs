using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using GreenT.HornyScapes;
using GreenT.Types;

namespace StripClub.Model.Shop;

public class Price<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
{
	public static Price<T> Free = new Price<T>(default(T), CurrencyType.Soft, default(CompositeIdentificator));

	public T Value { get; set; }

	public CurrencyType Currency { get; }

	public CompositeIdentificator CompositeIdentificator { get; }

	public Price(T price, CurrencyType currency, CompositeIdentificator compositeIdentificator)
	{
		Value = price;
		Currency = currency;
		CompositeIdentificator = compositeIdentificator;
	}

	public override string ToString()
	{
		if (Currency == CurrencyType.Real)
		{
			if (PlatformHelper.IsNutakuMonetization())
			{
				return Value.ToString() + " <sprite=1>";
			}
			if (PlatformHelper.IsErolabsMonetization())
			{
				return Value.ToString() + " <sprite=4>";
			}
			return $"${Value}";
		}
		return Value.ToString();
	}

	public static bool TryParse(string s, out Price<T> price)
	{
		MatchCollection matchCollection = new Regex("\\w+").Matches(s);
		if (matchCollection.Count != 2 || !TryConvert(matchCollection[0].Value, out var value) || !Enum.TryParse<CurrencyType>(matchCollection[1].Value, out var result))
		{
			price = null;
			return false;
		}
		price = new Price<T>(value, result, default(CompositeIdentificator));
		return true;
	}

	private static bool TryConvert(string input, out T value)
	{
		try
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter != null)
			{
				value = (T)converter.ConvertFromString(input);
				return true;
			}
			value = default(T);
			return false;
		}
		catch (NotSupportedException)
		{
			value = default(T);
			return false;
		}
	}
}

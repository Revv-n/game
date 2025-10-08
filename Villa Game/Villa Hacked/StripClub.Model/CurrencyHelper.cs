namespace StripClub.Model;

public static class CurrencyHelper
{
	public static bool IsRealCurrency(this CurrencyType currencyType)
	{
		return currencyType == CurrencyType.Real;
	}

	public static bool IsEventCurrency(this CurrencyType currencyType)
	{
		if (currencyType != CurrencyType.Event)
		{
			return currencyType == CurrencyType.EventXP;
		}
		return true;
	}

	public static bool IsInGameCurrency(this CurrencyType currencyType)
	{
		return currencyType != CurrencyType.Real;
	}
}

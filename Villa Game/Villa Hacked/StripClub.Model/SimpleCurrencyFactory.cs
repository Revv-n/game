using System.Linq;
using GreenT.Data;
using GreenT.Types;
using StripClub.Model.Data;
using Zenject;

namespace StripClub.Model;

public class SimpleCurrencyFactory : IFactory<CurrencyType, int, SimpleCurrency>, IFactory, IFactory<CurrencyType, int, string, int[], SimpleCurrency>
{
	private readonly ISaver saver;

	private readonly Currencies currencies;

	public SimpleCurrencyFactory(ISaver saver, Currencies currencies)
	{
		this.saver = saver;
		this.currencies = currencies;
	}

	public SimpleCurrency Create(CurrencyType type, int value)
	{
		return Create(type, value, null, null);
	}

	private SimpleCurrency CreateCurrency(CurrencyType type, int value, string saveKey = null, params int[] identificators)
	{
		return new SimpleCurrency(type, value, saveKey, identificators);
	}

	public SimpleCurrency Create(CurrencyType type, int value, string saveKey, params int[] identificators)
	{
		if (identificators == null || !identificators.Any())
		{
			identificators = new int[1];
		}
		CompositeIdentificator identificator = new CompositeIdentificator(identificators);
		if (currencies.Contains(type, identificator) && type != CurrencyType.BP && type != CurrencyType.Energy)
		{
			return currencies[type, identificator];
		}
		SimpleCurrency simpleCurrency = CreateCurrency(type, value, saveKey, identificators);
		saver.Add(simpleCurrency);
		if (type != CurrencyType.MiniEvent && type != CurrencyType.EventEnergy)
		{
			SimpleCurrency simpleCurrency2 = CreateCurrency(type, value, saveKey, identificators);
			simpleCurrency2.UpdateUniqueKeyToOld();
			saver.Add(simpleCurrency2);
			switch (type)
			{
			case CurrencyType.Energy:
				currencies.SetOldEnergy(simpleCurrency2);
				break;
			default:
				currencies.SetOld(simpleCurrency2);
				break;
			case CurrencyType.BP:
				currencies.SetOldBattlePass(simpleCurrency2);
				if (!simpleCurrency2.IsMigrated98aTo98fe)
				{
					simpleCurrency2.IsMigrated98aTo98fe = true;
					simpleCurrency.Migrate98aTo98fe(simpleCurrency2.Count.Value, simpleCurrency2.Identificator);
				}
				break;
			}
		}
		currencies.Set(type, simpleCurrency, simpleCurrency.Identificator);
		return simpleCurrency;
	}

	public void MigrateCurrencies()
	{
		currencies.ForceMigrate();
	}
}

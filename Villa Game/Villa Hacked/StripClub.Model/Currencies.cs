using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GreenT.HornyScapes;
using GreenT.Types;
using StripClub.Model.Data;
using UniRx;
using UnityEngine;
using Zenject;

namespace StripClub.Model;

[Serializable]
public class Currencies : ISerializable
{
	[Inject]
	private CurrenciesService _currenciesService;

	private SimpleCurrency.CurrencyKey _commonCurrencyKey;

	private Subject<(CurrencyType, SimpleCurrency)> _newCurrency = new Subject<(CurrencyType, SimpleCurrency)>();

	public SimpleCurrencyDictionary CurrenciesDict = new SimpleCurrencyDictionary();

	public List<SimpleCurrency> OldCurrenciesDict = new List<SimpleCurrency>();

	public SimpleCurrency OldEnergy;

	public SimpleCurrency OldBattlePass;

	private const string MIGRATE_93_TO_94_KEY = "IsMigrateFrom93To94";

	public IObservable<(CurrencyType, SimpleCurrency)> OnNewCurrency => (IObservable<(CurrencyType, SimpleCurrency)>)_newCurrency;

	public SimpleCurrency this[CurrencyType currencyType, CompositeIdentificator identificator = default(CompositeIdentificator)] => AlternativeGet(currencyType, identificator);

	private static CurrencyType[] CurrencyTypes => (CurrencyType[])Enum.GetValues(typeof(CurrencyType));

	public void SetOldBattlePass(SimpleCurrency simpleCurrency)
	{
		OldBattlePass = simpleCurrency;
	}

	public void SetOldEnergy(SimpleCurrency simpleCurrency)
	{
		OldEnergy = simpleCurrency;
	}

	public void SetOld(SimpleCurrency simpleCurrency)
	{
		if (simpleCurrency.CurrencyType != CurrencyType.MiniEvent)
		{
			SimpleCurrency simpleCurrency2 = OldCurrenciesDict.FirstOrDefault((SimpleCurrency currency) => currency.UniqueKey() == simpleCurrency.UniqueKey() && currency.Identificator == simpleCurrency.Identificator);
			if (simpleCurrency2 != null)
			{
				OldCurrenciesDict.Remove(simpleCurrency2);
			}
			OldCurrenciesDict.Add(simpleCurrency);
		}
	}

	public void ForceMigrate()
	{
		int i;
		for (i = 0; i < OldCurrenciesDict.Count; i++)
		{
			SimpleCurrency simpleCurrency = CurrenciesDict.Values.FirstOrDefault((SimpleCurrency value) => value.GetOldUniqueKey() == OldCurrenciesDict[i].UniqueKey());
			if (simpleCurrency != null && !simpleCurrency.IsMigrated98aTo98fe)
			{
				if (OldCurrenciesDict[i].Count.Value == OldCurrenciesDict[i].InitialValue)
				{
					simpleCurrency.IsMigrated98aTo98fe = true;
				}
				else
				{
					simpleCurrency.Migrate98aTo98fe(OldCurrenciesDict[i].Count.Value, OldCurrenciesDict[i].Identificator);
				}
			}
		}
	}

	public void Set(CurrencyType type, SimpleCurrency simpleCurrency, CompositeIdentificator identificator = default(CompositeIdentificator))
	{
		SetupCommonCurrencyKey(type, identificator);
		CurrenciesDict[_commonCurrencyKey] = simpleCurrency;
		_newCurrency.OnNext((type, CurrenciesDict[_commonCurrencyKey]));
	}

	public bool Contains(CurrencyType type, CompositeIdentificator identificator)
	{
		SetupCommonCurrencyKey(type, identificator);
		return CurrenciesDict.ContainsKey(_commonCurrencyKey);
	}

	public bool IsEnough(Cost cost)
	{
		return IsEnough(cost.Currency, cost.Amount);
	}

	public bool IsEnough(CurrencyType currencyType, int value, CompositeIdentificator identificator = default(CompositeIdentificator))
	{
		SetupCommonCurrencyKey(currencyType, identificator);
		if (!CurrenciesDict.TryGetValue(_commonCurrencyKey, out var value2))
		{
			return false;
		}
		return value2.Count.Value >= value;
	}

	public bool TryAdd(Cost cost)
	{
		return TryAdd(cost.Currency, cost.Amount);
	}

	public bool TryAdd(CurrencyType currencyType, int value, CompositeIdentificator identificator = default(CompositeIdentificator))
	{
		_currenciesService.CheckoutCurrency(currencyType, identificator);
		SetupCommonCurrencyKey(currencyType, identificator);
		if (value <= 0)
		{
			return false;
		}
		if (!CurrenciesDict.TryGetValue(_commonCurrencyKey, out var _))
		{
			return false;
		}
		if (currencyType != CurrencyType.MiniEvent)
		{
			ReactiveProperty<int> count = OldCurrenciesDict.FirstOrDefault((SimpleCurrency value) => value.UniqueKey() == CurrenciesDict[_commonCurrencyKey].GetOldUniqueKey() && value.Identificator == CurrenciesDict[_commonCurrencyKey].Identificator).Count;
			count.Value += value;
		}
		ReactiveProperty<int> count2 = CurrenciesDict[_commonCurrencyKey].Count;
		count2.Value += value;
		return true;
	}

	public bool Spend(Cost cost)
	{
		return Spend(cost.Currency, cost.Amount);
	}

	public bool Spend(CurrencyType currencyType, int value, CompositeIdentificator identificator = default(CompositeIdentificator))
	{
		_currenciesService.CheckoutCurrency(currencyType, identificator);
		SetupCommonCurrencyKey(currencyType, identificator);
		if (!IsEnough(currencyType, value, identificator))
		{
			return false;
		}
		if (currencyType != CurrencyType.MiniEvent)
		{
			ReactiveProperty<int> count = OldCurrenciesDict.FirstOrDefault((SimpleCurrency value) => value.UniqueKey() == CurrenciesDict[_commonCurrencyKey].GetOldUniqueKey() && value.Identificator == CurrenciesDict[_commonCurrencyKey].Identificator).Count;
			count.Value -= value;
		}
		ReactiveProperty<int> count2 = CurrenciesDict[_commonCurrencyKey].Count;
		count2.Value -= value;
		return true;
	}

	public void Reset(CurrencyType currencyType, CompositeIdentificator identificator = default(CompositeIdentificator))
	{
		_currenciesService.CheckoutCurrency(currencyType, identificator);
		SetupCommonCurrencyKey(currencyType, identificator);
		if (currencyType != CurrencyType.MiniEvent)
		{
			OldCurrenciesDict.FirstOrDefault((SimpleCurrency value) => value.UniqueKey() == CurrenciesDict[_commonCurrencyKey].GetOldUniqueKey() && value.Identificator == CurrenciesDict[_commonCurrencyKey].Identificator).Count.Value = 0;
		}
		CurrenciesDict[_commonCurrencyKey].Count.Value = 0;
	}

	public ReactiveProperty<int> Get(CurrencyType type, CompositeIdentificator identificator = default(CompositeIdentificator))
	{
		_currenciesService.CheckoutCurrency(type, identificator);
		SetupCommonCurrencyKey(type, identificator);
		return CurrenciesDict[_commonCurrencyKey].Count;
	}

	private SimpleCurrency.CurrencyKey GetCurrencyKey(CurrencyType currencyType, CompositeIdentificator identificator = default(CompositeIdentificator))
	{
		SetupCommonCurrencyKey(currencyType, identificator);
		return _commonCurrencyKey;
	}

	private void SetupCommonCurrencyKey(CurrencyType currencyType, CompositeIdentificator identificator)
	{
		_commonCurrencyKey.CurrencyType = currencyType;
		if (identificator.Identificators == null)
		{
			identificator = new CompositeIdentificator(default(int));
		}
		_commonCurrencyKey.Identificator = identificator;
	}

	private SimpleCurrency AlternativeGet(CurrencyType currencyType, CompositeIdentificator identificator)
	{
		_currenciesService.CheckoutCurrency(currencyType, identificator);
		return CurrenciesDict[GetCurrencyKey(currencyType, identificator)];
	}

	public Currencies()
	{
	}

	public Currencies(SerializationInfo info, StreamingContext context, CurrenciesService currenciesService)
		: this()
	{
		_currenciesService = currenciesService;
		if (WasMigrateOrNewPlayer())
		{
			return;
		}
		CurrencyType[] currencyTypes = CurrencyTypes;
		for (int i = 0; i < currencyTypes.Length; i++)
		{
			CurrencyType currencyType = currencyTypes[i];
			int num = 0;
			try
			{
				num = (int)info.GetValue(currencyType.ToString(), typeof(int));
			}
			catch (SerializationException)
			{
				Debug.Log("TUTA");
			}
			SimpleCurrency.CurrencyKey currencyKey = GetCurrencyKey(currencyType);
			if (!CurrenciesDict.Keys.Contains(currencyKey))
			{
				CurrenciesDict[currencyKey] = new SimpleCurrency(currencyType, num, "");
			}
			else
			{
				CurrenciesDict[currencyKey].Count.Value = num;
			}
		}
		bool WasMigrateOrNewPlayer()
		{
			bool result = false;
			try
			{
				result = (bool)info.GetValue("IsMigrateFrom93To94", typeof(bool));
			}
			catch (Exception)
			{
			}
			return result;
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue("IsMigrateFrom93To94", value: true);
	}
}

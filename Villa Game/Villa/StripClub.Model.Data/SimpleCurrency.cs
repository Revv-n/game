using System;
using System.Linq;
using GreenT.Data;
using GreenT.Types;
using UniRx;

namespace StripClub.Model.Data;

[Serializable]
[MementoHolder]
public class SimpleCurrency : ISavableState
{
	[Serializable]
	public struct CurrencyKey
	{
		public CurrencyType CurrencyType;

		public CompositeIdentificator Identificator;

		public CurrencyKey(CurrencyType currencyType, CompositeIdentificator identificator)
		{
			CurrencyType = currencyType;
			Identificator = identificator;
		}
	}

	[Serializable]
	public class CurrencyMemento : Memento
	{
		public int Count;

		public CompositeIdentificator Identificator;

		public bool IsMigrated98aTo98fe;

		public CurrencyMemento(SimpleCurrency savableState)
			: base(savableState)
		{
			Count = savableState.Count.Value;
			Identificator = savableState.Identificator;
			IsMigrated98aTo98fe = savableState.IsMigrated98aTo98fe;
		}
	}

	private string uniqueKey;

	public int InitialValue;

	public bool IsMigrated98aTo98fe;

	public CompositeIdentificator Identificator { get; private set; }

	public CurrencyType CurrencyType { get; private set; }

	public ReactiveProperty<int> Count { get; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public SimpleCurrency(CurrencyType currencyType, int initialValue, string saveKey = "", params int[] identificators)
	{
		InitialValue = initialValue;
		CurrencyType = currencyType;
		SetupIdentificator(identificators);
		Count = new ReactiveProperty<int>(initialValue);
		if (!string.IsNullOrEmpty(saveKey))
		{
			saveKey += "_";
		}
		uniqueKey = saveKey + "currency_" + currencyType.ToString() + Identificator.ToString();
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public void Initialize()
	{
		Count.Value = InitialValue;
	}

	public Memento SaveState()
	{
		return new CurrencyMemento(this);
	}

	public void LoadState(Memento memento)
	{
		CurrencyMemento currencyMemento = (CurrencyMemento)memento;
		Count.Value = currencyMemento.Count;
		IsMigrated98aTo98fe = currencyMemento.IsMigrated98aTo98fe;
		if (currencyMemento.Identificator.Identificators != null && currencyMemento.Identificator.Identificators.Any())
		{
			Identificator = currencyMemento.Identificator;
		}
	}

	private void SetupIdentificator(params int[] identificators)
	{
		if (identificators == null || !identificators.Any())
		{
			Identificator = new CompositeIdentificator(default(int));
		}
		else
		{
			Identificator = new CompositeIdentificator(identificators);
		}
	}

	public string GetOldUniqueKey()
	{
		return uniqueKey.Substring(0, uniqueKey.Length - Identificator.ToString().Length);
	}

	public void UpdateUniqueKeyToOld()
	{
		uniqueKey = GetOldUniqueKey();
	}

	public void Migrate98aTo98fe(int count, CompositeIdentificator identificator)
	{
		IsMigrated98aTo98fe = true;
		Count.Value = count;
		Identificator = identificator;
	}
}

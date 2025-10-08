using System;
using GreenT.Data;

namespace GreenT.HornyScapes.BattlePassSpace;

[MementoHolder]
public class BattlePassMergedCurrencyDataCase : ISavableState
{
	[Serializable]
	public class MementoData : Memento
	{
		public int MergedCurrencyQuantity { get; private set; }

		public MementoData(BattlePassMergedCurrencyDataCase dataCase)
			: base(dataCase)
		{
			MergedCurrencyQuantity = dataCase.MergedCurrencyQuantity;
		}
	}

	public int MergedCurrencyQuantity;

	private readonly string _saveKey;

	public SavableStatePriority Priority { get; } = SavableStatePriority.Base;


	public BattlePassMergedCurrencyDataCase(string saveKey)
	{
		_saveKey = saveKey;
	}

	public void Reset()
	{
		MergedCurrencyQuantity = 0;
	}

	public string UniqueKey()
	{
		return _saveKey;
	}

	public Memento SaveState()
	{
		return new MementoData(this);
	}

	public void LoadState(Memento memento)
	{
		MergedCurrencyQuantity = ((MementoData)memento).MergedCurrencyQuantity;
	}
}

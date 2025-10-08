using System;
using GreenT.Data;

namespace GreenT.HornyScapes;

[MementoHolder]
public class SavableVariable<T> : ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public T Value { get; private set; }

		public Memento(SavableVariable<T> variable)
			: base(variable)
		{
			Value = variable.Value;
		}
	}

	protected string variableKey;

	public T Value { get; set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public SavableVariable(string key, T initValue = default(T))
	{
		variableKey = key;
		Value = initValue;
	}

	public string UniqueKey()
	{
		return variableKey;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		Value = memento2.Value;
	}
}

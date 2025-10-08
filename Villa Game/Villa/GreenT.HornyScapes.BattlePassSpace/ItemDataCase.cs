using System;
using System.Collections.Generic;
using GreenT.Data;

namespace GreenT.HornyScapes.BattlePassSpace;

[MementoHolder]
public class ItemDataCase : ISavableState
{
	[Serializable]
	public class ItemDataCaseMemento : Memento
	{
		public List<SaveInfo> LoadInfo = new List<SaveInfo>();

		public ItemDataCaseMemento(ItemDataCase savableState)
			: base(savableState)
		{
		}
	}

	[Serializable]
	public struct SaveInfo
	{
		public int X;

		public int Y;
	}

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public string UniqueKey()
	{
		return "";
	}

	public Memento SaveState()
	{
		return new ItemDataCaseMemento(this);
	}

	public void LoadState(Memento memento)
	{
	}
}

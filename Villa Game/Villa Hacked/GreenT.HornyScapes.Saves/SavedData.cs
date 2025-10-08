using System;
using System.Collections.Generic;
using GreenT.Data;

namespace GreenT.HornyScapes.Saves;

[Serializable]
public class SavedData
{
	public long UpdatedAt;

	public List<Memento> Data;

	public SavedData(long updatedAt, List<Memento> data)
	{
		UpdatedAt = updatedAt;
		Data = data;
	}

	public SavedData()
	{
	}

	public void ClearDummies()
	{
		for (int num = Data.Count - 1; num >= 0; num--)
		{
			if (Data[num].GetType() == typeof(MigrationDummyMemento))
			{
				Data.RemoveAt(num);
			}
		}
	}
}

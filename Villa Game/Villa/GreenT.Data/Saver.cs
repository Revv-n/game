using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.Saves;

namespace GreenT.Data;

public sealed class Saver : ISaver
{
	private class CustomSortList
	{
		private readonly Dictionary<SavableStatePriority, HashSet<ISavableState>> savableStates = new Dictionary<SavableStatePriority, HashSet<ISavableState>>();

		private List<ISavableState> savableStatesList = new List<ISavableState>();

		private bool actual;

		private int count;

		public void Add(ISavableState savableState)
		{
			if (!savableStates.ContainsKey(savableState.Priority))
			{
				savableStates.Add(savableState.Priority, new HashSet<ISavableState>());
			}
			HashSet<ISavableState> hashSet = savableStates[savableState.Priority];
			if (!hashSet.Contains(savableState))
			{
				hashSet.Add(savableState);
				IncreaseCount(1);
			}
		}

		public bool Remove(ISavableState savableState)
		{
			if (!savableStates.ContainsKey(savableState.Priority))
			{
				return false;
			}
			HashSet<ISavableState> hashSet = savableStates[savableState.Priority];
			if (!hashSet.Contains(savableState))
			{
				return false;
			}
			hashSet.Remove(savableState);
			IncreaseCount(-1);
			return true;
		}

		private void IncreaseCount(int value)
		{
			count += value;
			actual = false;
		}

		public List<ISavableState> GetList()
		{
			if (actual)
			{
				return savableStatesList;
			}
			savableStatesList.Clear();
			savableStatesList = new List<ISavableState>(count);
			Array values = Enum.GetValues(typeof(SavableStatePriority));
			Array.Reverse(values);
			foreach (SavableStatePriority item in values)
			{
				if (savableStates.ContainsKey(item))
				{
					savableStatesList.AddRange(savableStates[item]);
				}
			}
			actual = true;
			return savableStatesList;
		}
	}

	private CustomSortList savableListNew = new CustomSortList();

	private readonly Hashtable hashtable = new Hashtable();

	public bool SuppressSaving { get; private set; }

	public void SetSuppressState(bool state)
	{
		SuppressSaving = state;
	}

	public void Add(ISavableState savable)
	{
		savableListNew.Add(savable);
		Memento mementoOrDefault = GetMementoOrDefault(savable);
		if (mementoOrDefault != null)
		{
			savable.LoadState(mementoOrDefault);
		}
	}

	public bool Remove(ISavableState savable)
	{
		return savableListNew.Remove(savable);
	}

	public bool Delete(ISavableState savable)
	{
		string key = savable.UniqueKey();
		hashtable.Remove(key);
		return !Remove(savable);
	}

	public void DeleteHashTablePoint(string uniqKey)
	{
		if (hashtable.ContainsKey(uniqKey))
		{
			hashtable.Remove(uniqKey);
		}
	}

	public void LoadState(SavedData state)
	{
		if (state != null)
		{
			LoadState(state.Data, savableListNew.GetList());
		}
	}

	public void LoadState(List<Memento> state, List<ISavableState> savableObjets)
	{
		if (state == null)
		{
			return;
		}
		foreach (Memento item in state)
		{
			hashtable[item.UniqueKey] = item;
		}
		for (int i = 0; i != savableObjets.Count; i++)
		{
			LoadState(savableObjets[i]);
		}
	}

	public void ClearStates()
	{
		hashtable.Clear();
	}

	public List<Memento> GetState()
	{
		foreach (ISavableState item in savableListNew.GetList())
		{
			SaveCurrentState(item);
		}
		return hashtable.Values.OfType<Memento>().ToList();
	}

	private Memento GetMementoOrDefault(ISavableState savable)
	{
		string key = savable.UniqueKey();
		return (Memento)hashtable[key];
	}

	public bool TryGetMemento(string uniqKey, out Memento memento)
	{
		if (hashtable.ContainsKey(uniqKey))
		{
			memento = (Memento)hashtable[uniqKey];
		}
		else
		{
			memento = null;
		}
		return memento != null;
	}

	private void SaveCurrentState(ISavableState savable)
	{
		Memento memento = savable.SaveState();
		hashtable[memento.UniqueKey] = memento;
	}

	private void LoadState(ISavableState savable)
	{
		string key = savable.UniqueKey();
		object obj = hashtable[key];
		if (obj != null)
		{
			savable.LoadState((Memento)obj);
		}
	}
}

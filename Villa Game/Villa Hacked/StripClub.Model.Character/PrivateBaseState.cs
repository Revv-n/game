using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;

namespace StripClub.Model.Character;

[MementoHolder]
public class PrivateBaseState : ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public int[] unlockedCharacterIDs;

		public Memento(PrivateBaseState privateBaseState)
			: base(privateBaseState)
		{
			unlockedCharacterIDs = privateBaseState.UnlockedCharacterIDs.ToArray();
		}
	}

	private int[] loadedIDs = new int[0];

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public IPrivateInfoBase Source { get; private set; }

	public IEnumerable<int> UnlockedCharacterIDs => (from _char in Source.GetEverybodiesInfo()
		select _char.Public.ID).Union(loadedIDs);

	public string UniqueKey()
	{
		return "InfoBase";
	}

	public PrivateBaseState(IPrivateInfoBase infoBase)
	{
		Source = infoBase;
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		loadedIDs = memento2.unlockedCharacterIDs;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Model.Character;

namespace GreenT.HornyScapes.Characters;

[MementoHolder]
public class CharacterManagerState : ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public int[] unlockedCharacterIDs;

		public Memento(CharacterManagerState privateBaseState)
			: base(privateBaseState)
		{
			unlockedCharacterIDs = privateBaseState.UnlockedCharacterIDs.ToArray();
			privateBaseState.Source.Collection.Select((CharacterSettings _char) => _char.Public.ID).ToArray();
			_ = privateBaseState.loadedIDs;
		}
	}

	private int[] loadedIDs = new int[0];

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public CharacterSettingsManager Source { get; private set; }

	public IEnumerable<int> UnlockedCharacterIDs => Source.Collection.Select((CharacterSettings _char) => _char.Public.ID).Union(loadedIDs);

	public string UniqueKey()
	{
		return "InfoBase";
	}

	public CharacterManagerState(CharacterSettingsManager infoBase)
	{
		Source = infoBase;
	}

	public void Initialize()
	{
		loadedIDs = new int[0];
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		if (memento is Memento memento2)
		{
			loadedIDs = memento2.unlockedCharacterIDs;
		}
		else if (memento is PrivateBaseState.Memento memento3)
		{
			loadedIDs = memento3.unlockedCharacterIDs;
		}
	}

	public void Distinct()
	{
		loadedIDs = loadedIDs.Except(Source.Collection.Select((CharacterSettings _char) => _char.Public.ID)).ToArray();
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}
}

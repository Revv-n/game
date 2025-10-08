using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Model.Cards;
using StripClub.Model.Data;

namespace StripClub.Model.Character;

[Serializable]
public class CharacterConfiguration : ISavableState
{
	private const string uniqueKeyPrefix = "character_";

	private readonly string uniqueKey;

	public List<int> ownedSkinIDs;

	private int avatarNumber;

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public ICard Public { get; private set; }

	public int SkinID { get; private set; } = -1;


	public IEnumerable<int> OwnedSkinIDs => ownedSkinIDs.AsEnumerable();

	public int AvatarNumber => avatarNumber;

	public IPromote Promote { get; protected set; }

	public event EventHandler OnUpdate;

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public CharacterConfiguration(ICard generalInfo, IPromote promote)
	{
		Public = generalInfo;
		ownedSkinIDs = new List<int>();
		avatarNumber = 0;
		Promote = promote;
		uniqueKey = "character_" + Public.ID;
	}

	public Memento SaveState()
	{
		return new CharacterProgressMemento(this);
	}

	public void LoadState(Memento memento)
	{
		if (memento is CharacterProgressMemento)
		{
			CharacterProgressMemento characterProgressMemento = (CharacterProgressMemento)memento;
			ownedSkinIDs.AddRange(characterProgressMemento.OwnedSkinIDs);
			SkinID = characterProgressMemento.SkinID;
			Promote.Init(characterProgressMemento.Level, characterProgressMemento.Progress);
			Promote.IsNew.Value = characterProgressMemento.IsNew;
			avatarNumber = characterProgressMemento.AvatarNumber;
		}
	}
}

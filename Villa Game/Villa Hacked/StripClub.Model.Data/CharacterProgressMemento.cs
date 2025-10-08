using System;
using System.Linq;
using GreenT.Data;
using StripClub.Model.Character;
using UnityEngine;

namespace StripClub.Model.Data;

[Serializable]
[MementoHolder]
public class CharacterProgressMemento : Memento
{
	[SerializeField]
	private int id;

	[SerializeField]
	private int skinID;

	[SerializeField]
	private int[] skinMask;

	[SerializeField]
	private int energy;

	[SerializeField]
	private int level;

	[SerializeField]
	private int progress;

	[SerializeField]
	private int avatarNumber;

	[SerializeField]
	private bool isNew;

	[SerializeField]
	private int promoteProgress;

	[SerializeField]
	private int promoteLevel;

	public int ID
	{
		get
		{
			return id;
		}
		private set
		{
			id = value;
		}
	}

	public int SkinID
	{
		get
		{
			return skinID;
		}
		private set
		{
			skinID = value;
		}
	}

	public int[] OwnedSkinIDs
	{
		get
		{
			return skinMask;
		}
		private set
		{
			skinMask = value;
		}
	}

	public int Energy
	{
		get
		{
			return energy;
		}
		private set
		{
			energy = value;
		}
	}

	public int AvatarNumber
	{
		get
		{
			return avatarNumber;
		}
		private set
		{
			avatarNumber = value;
		}
	}

	public bool IsNew
	{
		get
		{
			return isNew;
		}
		private set
		{
			isNew = value;
		}
	}

	public int Level
	{
		get
		{
			return level;
		}
		private set
		{
			level = value;
		}
	}

	public int Progress
	{
		get
		{
			return progress;
		}
		private set
		{
			progress = value;
		}
	}

	public CharacterProgressMemento(CharacterConfiguration info)
		: base(info)
	{
		ID = info.Public.ID;
		SkinID = info.SkinID;
		AvatarNumber = info.AvatarNumber;
		OwnedSkinIDs = info.OwnedSkinIDs.ToArray();
		Level = info.Promote.Level.Value;
		Progress = info.Promote.Progress.Value;
		IsNew = info.Promote.IsNew.Value;
	}
}

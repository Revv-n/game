using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;

namespace StripClub.Gallery;

[MementoHolder]
public class MediaInfo : IMediaInfo, ISavableState
{
	[Serializable]
	public class Memento : GreenT.Data.Memento
	{
		public bool favourite;

		public Memento(MediaInfo info)
			: base(info)
		{
			favourite = info.Favourite;
		}
	}

	private readonly string uniqueKey;

	public int ID { get; private set; }

	public Type Type { get; private set; }

	public IEnumerable<int> CharacterIDs { get; private set; }

	public IEnumerable<int> tagIDs { get; private set; }

	public bool Favourite { get; set; }

	public SavableStatePriority Priority => SavableStatePriority.Base;

	public MediaInfo(int id, Type type, IEnumerable<int> characterIDs, IEnumerable<int> tagIDs)
	{
		ID = id;
		Type = type;
		CharacterIDs = characterIDs;
		this.tagIDs = tagIDs;
		uniqueKey = "media_" + ID;
	}

	private bool IsMatch(IEnumerable<int> otherIDs)
	{
		if (otherIDs.Except(tagIDs).Any())
		{
			return false;
		}
		return true;
	}

	public bool IsMatch(MediaFilter filter)
	{
		if (filter.Favourites && !Favourite)
		{
			return false;
		}
		if (filter.Type != null && filter.Type != Type)
		{
			return false;
		}
		if (filter.CharIDs != null && filter.CharIDs.Except(CharacterIDs).Any())
		{
			return false;
		}
		if (filter.TagIDs != null && !IsMatch(filter.TagIDs))
		{
			return false;
		}
		return true;
	}

	public string UniqueKey()
	{
		return uniqueKey;
	}

	public GreenT.Data.Memento SaveState()
	{
		return new Memento(this);
	}

	public void LoadState(GreenT.Data.Memento memento)
	{
		Memento memento2 = (Memento)memento;
		Favourite = memento2.favourite;
	}
}

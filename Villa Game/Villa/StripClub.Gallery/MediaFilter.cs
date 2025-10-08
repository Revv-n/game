using System;
using System.Collections.Generic;
using System.Linq;

namespace StripClub.Gallery;

public class MediaFilter
{
	public IEnumerable<int> CharIDs { get; private set; }

	public Type Type { get; private set; }

	public IEnumerable<int> TagIDs { get; private set; }

	public bool Favourites { get; set; }

	public MediaFilter WhereChar(int characterID)
	{
		CharIDs = new int[1] { characterID };
		return this;
	}

	public MediaFilter WhereChar(IEnumerable<int> characterIDs)
	{
		CharIDs = characterIDs?.ToArray();
		return this;
	}

	public MediaFilter WhereTags(IEnumerable<int> tagIDs)
	{
		TagIDs = ((tagIDs == null) ? null : tagIDs);
		return this;
	}

	public MediaFilter WhereType(Type type)
	{
		Type = type;
		return this;
	}

	public MediaFilter WhereFavourites(bool favourites = true)
	{
		Favourites = favourites;
		return this;
	}

	public MediaFilter Purge()
	{
		CharIDs = null;
		Type = null;
		TagIDs = null;
		Favourites = false;
		return this;
	}
}

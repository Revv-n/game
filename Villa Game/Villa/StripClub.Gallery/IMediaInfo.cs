using System;
using System.Collections.Generic;

namespace StripClub.Gallery;

public interface IMediaInfo
{
	int ID { get; }

	Type Type { get; }

	IEnumerable<int> CharacterIDs { get; }

	IEnumerable<int> tagIDs { get; }

	bool Favourite { get; set; }

	bool IsMatch(MediaFilter filter);
}

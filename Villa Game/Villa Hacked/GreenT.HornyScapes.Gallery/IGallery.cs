using System;
using System.Collections.Generic;
using StripClub.Gallery;

namespace GreenT.HornyScapes.Gallery;

public interface IGallery
{
	IObservable<Media> OnMediaAdded { get; }

	IObservable<Media> OnMediaReplaced { get; }

	bool TryGet(int mediaID, out Media media);

	IEnumerable<Media> GetMedia(MediaFilter filter = null);

	int Count(MediaFilter filter = null);

	IEnumerable<int> GetUniqueTagIDs();

	IEnumerable<int> GetUniqueCharacterIDs();

	void Purge();
}

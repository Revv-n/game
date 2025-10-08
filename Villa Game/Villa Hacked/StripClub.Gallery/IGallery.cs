using System;
using System.Collections.Generic;

namespace StripClub.Gallery;

public interface IGallery
{
	int MediaTotal { get; }

	event Action<IEnumerable<IMediaInfo>> OnUpdate;

	void Add(IEnumerable<IMediaInfo> info);

	IMediaInfo GetMediaInfo(int id);

	IEnumerable<int> GetUniqueCharacterIDs();

	IEnumerable<int> GetUniqueTagIDs();

	IEnumerable<IMediaInfo> GetMediaInfos(MediaFilter filter = null);

	bool TryGet(int mediaID, out Media media);

	IEnumerable<Media> GetMedia(MediaFilter filter = null);
}

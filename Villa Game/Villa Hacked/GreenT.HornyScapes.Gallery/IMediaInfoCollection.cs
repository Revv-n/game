using System.Collections.Generic;
using StripClub.Gallery;

namespace GreenT.HornyScapes.Gallery;

public interface IMediaInfoCollection
{
	bool TryGet(int id, out IMediaInfo mediaInfo);

	IEnumerable<IMediaInfo> Get(MediaFilter filter = null);

	int Count(MediaFilter filter = null);
}

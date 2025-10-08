using UnityEngine;

namespace StripClub.Gallery;

public abstract class Media
{
	public IMediaInfo Info { get; }

	public Sprite Thumbnail { get; set; }

	public Media(IMediaInfo mediaInfo, Sprite thumbnail)
	{
		Info = mediaInfo;
		Thumbnail = thumbnail;
	}
}

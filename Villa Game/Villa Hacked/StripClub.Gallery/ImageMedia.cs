using UnityEngine;

namespace StripClub.Gallery;

public class ImageMedia : Media
{
	public Sprite Image { get; set; }

	public ImageMedia(IMediaInfo mediaInfo, Sprite thumbnail, Sprite image)
		: base(mediaInfo, thumbnail)
	{
		Image = image;
	}
}

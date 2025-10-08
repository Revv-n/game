using StripClub.Gallery;
using StripClub.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GreenT.HornyScapes.Gallery;

public abstract class MediaView<T> : MonoView<T> where T : Media
{
	[SerializeField]
	protected Image thumbnailImage;

	public override void Set(T source)
	{
		base.Set(source);
		thumbnailImage.sprite = source.Thumbnail;
	}
}

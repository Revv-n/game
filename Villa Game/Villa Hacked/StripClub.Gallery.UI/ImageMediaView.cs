using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Gallery.UI;

public abstract class ImageMediaView : MonoBehaviour
{
	[SerializeField]
	protected Image thumbnailImage;

	public abstract void SetPhoto(ImageMedia media);
}

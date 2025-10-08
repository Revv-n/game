using UnityEngine;
using UnityEngine.UI;

namespace StripClub.Gallery.UI;

public class FullscreenPhoto : ImageMediaView
{
	[SerializeField]
	private Image image;

	public void SetActive(bool isActive)
	{
		base.gameObject.SetActive(isActive);
	}

	public override void SetPhoto(ImageMedia media)
	{
		image.sprite = media.Image;
	}
}

using System.Linq;
using StripClub.Gallery;
using StripClub.UI;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Gallery.UI;

public class MediaViewManager : MonoBehaviour
{
	private ViewManager<ImageMedia, PhotoView> photoViewManager;

	private ViewManager<LockedView> lockedViewManager;

	private IMediaInfoCollection mediaInfoBase;

	private IGallery gallery;

	[Inject]
	public void Init(IMediaInfoCollection mediaInfoBase, IGallery gallery, ViewManager<ImageMedia, PhotoView> photoViewManager, ViewManager<LockedView> lockedViewManager)
	{
		this.mediaInfoBase = mediaInfoBase;
		this.gallery = gallery;
		this.photoViewManager = photoViewManager;
		this.lockedViewManager = lockedViewManager;
	}

	public void HideAll()
	{
		photoViewManager.HideAll();
	}

	public void DisplayGallery()
	{
		photoViewManager.HideAll();
		MediaFilter filter = new MediaFilter().WhereType(typeof(Sprite));
		int num = 0;
		foreach (ImageMedia item in gallery.GetMedia(filter).Cast<ImageMedia>())
		{
			photoViewManager.Display(item);
			num++;
		}
		int num2 = mediaInfoBase.Get(filter).Count() - num;
		lockedViewManager.HideAll();
		for (int i = 0; i != num2; i++)
		{
			lockedViewManager.GetView();
		}
	}

	public void OnEnable()
	{
		DisplayGallery();
	}
}

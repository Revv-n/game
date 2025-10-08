using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Gallery;
using StripClub.Model;
using StripClub.Utility;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Gallery.UI;

public class ContentGrid : MonoBehaviour
{
	private IGallery gallery;

	private IMediaInfoCollection mediaInfoBase;

	private PhotoView.Manager photoViewManager;

	private LockedView.Manager lockedViewManager;

	private MediaFilter mediaFilter;

	private GalleryState galleryState;

	private GalleryController galleryController;

	private List<PhotoView> showenViews = new List<PhotoView>();

	private List<LockedView> lockedViews = new List<LockedView>();

	private bool isLoadHD;

	public IEnumerable<Media> CurrentMedia { get; private set; }

	[Inject]
	internal void Init(GalleryController galleryController, GalleryState galleryState, IGallery gallery, IMediaInfoCollection mediaInfoBase, PhotoView.Manager photoViewManager, LockedView.Manager lockedViewManager)
	{
		this.galleryController = galleryController;
		this.galleryState = galleryState;
		this.gallery = gallery;
		this.photoViewManager = photoViewManager;
		this.lockedViewManager = lockedViewManager;
		this.mediaInfoBase = mediaInfoBase;
	}

	private void Awake()
	{
		Type type = StripClub.Utility.Content.ToType(MediaType.Photos);
		mediaFilter = new MediaFilter().WhereType(type);
		LoadMedia().Subscribe(ShowPhotoOnLoad).AddTo(this);
	}

	private IObservable<ImageMedia> LoadMedia()
	{
		return (from _id in galleryState.GainedMediaIDs.ToObservable()
			select from _media in galleryController.GetOrLoadMedia(_id)
				select _media as ImageMedia).Concat();
	}

	private void ShowPhotoOnLoad(ImageMedia _imageMedia)
	{
		int count = showenViews.Count;
		LockedView lockedView = lockedViews.FirstOrDefault();
		if (lockedView != null)
		{
			lockedView.Display(display: false);
			lockedViews.Remove(lockedView);
		}
		PhotoView photoView = photoViewManager.Display(_imageMedia);
		photoView.transform.SetSiblingIndex(count);
		showenViews.Add(photoView);
	}

	public void StartLoadHDContent()
	{
		if (!isLoadHD)
		{
			LoadMedia().Subscribe(ShowPhotoOnLoad).AddTo(this);
			isLoadHD = true;
		}
	}

	public void DisplayContent()
	{
		photoViewManager.HideAll();
		showenViews.Clear();
		int num = 0;
		foreach (ImageMedia item in gallery.GetMedia(mediaFilter).Cast<ImageMedia>())
		{
			PhotoView photoView = photoViewManager.Display(item);
			photoView.transform.SetSiblingIndex(num);
			num++;
			showenViews.Add(photoView);
		}
		int num2 = mediaInfoBase.Get(mediaFilter).Count() - num;
		lockedViewManager.HideAll();
		lockedViews.Clear();
		for (int i = 0; i != num2; i++)
		{
			LockedView view = lockedViewManager.GetView();
			lockedViews.Add(view);
		}
	}
}

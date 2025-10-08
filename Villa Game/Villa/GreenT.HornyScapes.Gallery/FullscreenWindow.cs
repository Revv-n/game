using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.HornyScapes.UI;
using GreenT.UI;
using StripClub.Gallery;
using StripClub.Gallery.UI;
using StripClub.Model;
using StripClub.Utility;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Gallery;

public class FullscreenWindow : Window
{
	[SerializeField]
	private FullscreenPhoto FullscreenPhoto;

	[SerializeField]
	private FloatClampedProgress slideTimer;

	[SerializeField]
	private SlideShow slideShow;

	[SerializeField]
	private Toggle favouriteToggle;

	private ImageMedia currentMedia;

	private MediaFilter mediaFilter;

	private IGallery gallery;

	[Inject]
	public void Init(IGallery gallery)
	{
		Type type = StripClub.Utility.Content.ToType(MediaType.Photos);
		mediaFilter = new MediaFilter().WhereType(type);
		this.gallery = gallery;
	}

	private void Start()
	{
		favouriteToggle.Init(null, OnFavouriteSwitch);
	}

	private void OnFavouriteSwitch(Toggle favourite)
	{
		favourite.SetSelected(!favourite.IsSelected);
		currentMedia.Info.Favourite = favourite.IsSelected;
	}

	private void OnEnable()
	{
		slideShow.OnSwitch += SwitchSlideToNext;
		slideShow.Progress += SetSlideTimer;
		slideTimer.Init(0f, 1f, 0f);
	}

	private void OnDisable()
	{
		slideShow.OnSwitch -= SwitchSlideToNext;
		slideShow.Progress -= SetSlideTimer;
	}

	private void SetSlideTimer(float current, float target)
	{
		slideTimer.Init(current, target, 0f);
	}

	public void SwitchSlideToNext()
	{
		List<ImageMedia> list = gallery.GetMedia(mediaFilter).OfType<ImageMedia>().ToList();
		int num = list.IndexOf(currentMedia) + 1;
		if (num >= list.Count)
		{
			num = 0;
		}
		if (list[num] != null)
		{
			currentMedia = list[num];
			SetPhoto(currentMedia);
		}
	}

	public void SetPhoto(ImageMedia media)
	{
		currentMedia = media;
		favouriteToggle.SetSelected(currentMedia.Info.Favourite);
		FullscreenPhoto.SetPhoto(media);
	}
}

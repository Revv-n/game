using System;
using GreenT.UI;
using StripClub.Gallery;
using UnityEngine;

namespace GreenT.HornyScapes.Gallery.UI;

public class PhotoalbumWindow : Window
{
	public bool IsFavorites;

	[SerializeField]
	private ContentGrid Grid;

	[SerializeField]
	private FullscreenWindow FullscreenWindow;

	public override void Open()
	{
		base.Open();
		Grid.DisplayContent();
		Grid.StartLoadHDContent();
	}

	private void TagsList_OnSelectTag(object tagsList, EventArgs e)
	{
		Grid.DisplayContent();
	}

	public void ShowMedia(Media media)
	{
		if (media is ImageMedia photo)
		{
			FullscreenWindow.Open();
			FullscreenWindow.SetPhoto(photo);
			return;
		}
		throw new ArgumentOutOfRangeException("No behaviour for this type of media:" + media.GetType()?.ToString() + " with id: " + media.Info.ID);
	}
}

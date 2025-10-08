using System;
using GreenT.UI;
using StripClub.Gallery;
using StripClub.Gallery.UI;
using StripClub.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GreenT.HornyScapes.Gallery.UI;

public class PhotoView : MediaView<ImageMedia>
{
	internal class Factory : ViewFactory<ImageMedia, PhotoView>
	{
		public Factory(DiContainer diContainer, Transform objectContainer, PhotoView prefab)
			: base(diContainer, objectContainer, prefab)
		{
		}
	}

	internal class Manager : ViewManager<ImageMedia, PhotoView>
	{
	}

	[SerializeField]
	private UnityEngine.UI.Toggle favoriteToggle;

	[SerializeField]
	private Button Button;

	private FullscreenPhoto fullscreenPhoto;

	private IWindowsManager windowsOpener;

	private FullscreenWindow fullscreenWindow;

	private IDisposable clickStream;

	private IDisposable favoriteToggleStream;

	[Inject]
	public void Init(FullscreenPhoto fullscreenPhoto, IWindowsManager windowsOpener)
	{
		this.fullscreenPhoto = fullscreenPhoto;
		this.windowsOpener = windowsOpener;
	}

	private void Start()
	{
		fullscreenWindow = windowsOpener.Get<FullscreenWindow>();
	}

	public override void Set(ImageMedia media)
	{
		base.Set(media);
		SetupButton(media);
		SetupFavoriteToggle(media);
	}

	private void SetupFavoriteToggle(ImageMedia media)
	{
		favoriteToggleStream?.Dispose();
		favoriteToggle.SetIsOnWithoutNotify(media.Info.Favourite);
		favoriteToggleStream = (favoriteToggleStream = favoriteToggle.OnValueChangedAsObservable().Subscribe(delegate(bool _value)
		{
			media.Info.Favourite = _value;
		}).AddTo(this));
	}

	private void SetupButton(ImageMedia media)
	{
		clickStream?.Dispose();
		Button.interactable = true;
		clickStream = Button.OnClickAsObservable().Subscribe(delegate
		{
			OpenPhoto(media);
		}).AddTo(this);
	}

	private void OpenPhoto(ImageMedia media)
	{
		fullscreenPhoto.SetPhoto(media);
		fullscreenWindow.Open();
	}
}

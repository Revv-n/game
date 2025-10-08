using System;
using StripClub.Gallery;
using StripClub.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GreenT.HornyScapes.Gallery.UI;

public class MediaCount : MonoBehaviour
{
	[SerializeField]
	private LocalizedTextMeshPro mediaGauge;

	private IGallery gallery;

	private IMediaInfoCollection mediaInfoBase;

	[Inject]
	public void Init(IGallery gallery, IMediaInfoCollection mediaInfoBase)
	{
		this.gallery = gallery;
		this.mediaInfoBase = mediaInfoBase;
	}

	private void Start()
	{
		UpdateGauge();
		DisposableExtensions.AddTo<IDisposable>(ObservableExtensions.Subscribe<Media>(gallery.OnMediaAdded, (Action<Media>)delegate
		{
			UpdateGauge();
		}), (Component)this);
	}

	private void UpdateGauge()
	{
		int num = mediaInfoBase.Count();
		int num2 = gallery.Count();
		mediaGauge.SetArguments(num2, num);
	}
}

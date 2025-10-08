using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.AssetBundles.Scheduler;
using StripClub.Extensions;
using StripClub.Gallery;
using StripClub.Model;
using StripClub.Model.Data;
using UniRx;

namespace GreenT.HornyScapes.Gallery;

public sealed class GalleryController : IDisposable
{
	private readonly ILoader<IEnumerable<IMediaInfo>, IEnumerable<Media>> mediaLoader;

	private readonly ILoader<IEnumerable<int>, Media> mediaLoaderById;

	private readonly ICollectionSetter<Media> mediaSetter;

	private readonly IGallery gallery;

	private readonly IMediaInfoCollection mediaInfoBase;

	private List<int> loadFirstMedia = new List<int>();

	private IDisposable loadStream;

	public GalleryController(ILoader<IEnumerable<IMediaInfo>, IEnumerable<Media>> mediaLoader, ILoader<IEnumerable<int>, Media> mediaLoaderById, ICollectionSetter<Media> mediaSetter, IGallery gallery, IMediaInfoCollection mediaInfoBase)
	{
		this.mediaLoaderById = mediaLoaderById;
		this.mediaLoader = mediaLoader;
		this.mediaSetter = mediaSetter;
		this.gallery = gallery;
		this.mediaInfoBase = mediaInfoBase;
	}

	public void LoadMedia(IEnumerable<int> mediaIDs)
	{
		List<IMediaInfo> list = new List<IMediaInfo>();
		foreach (int mediaID in mediaIDs)
		{
			if (mediaInfoBase.TryGet(mediaID, out var mediaInfo))
			{
				list.Add(mediaInfo);
			}
		}
		IObservable<IEnumerable<Media>> source = mediaLoader.Load(list).Share();
		loadStream = source.Subscribe(delegate(IEnumerable<Media> _medias)
		{
			mediaSetter.Add(_medias.ToArray());
		});
	}

	public IObservable<Media> GetOrLoadMedia(int mediaID)
	{
		if (gallery.TryGet(mediaID, out var media) && !CheckReloadMedia(mediaID))
		{
			return Observable.Return(media);
		}
		if (!mediaInfoBase.TryGet(mediaID, out var mediaInfo))
		{
			return mediaLoaderById.Load(mediaID.AsEnumerable()).First().Do(delegate(Media _media)
			{
				mediaSetter.Add(_media);
			});
		}
		return (from _media in mediaLoader.Load(mediaInfo.AsEnumerable())
			select _media.ToArray()).Do(mediaSetter.Add).SelectMany((Media[] x) => x);
	}

	private bool CheckReloadMedia(int mediaID)
	{
		if (loadFirstMedia.Contains(mediaID))
		{
			return MediaQuality.CheckSD(mediaID);
		}
		loadFirstMedia.Add(mediaID);
		return false;
	}

	public void Dispose()
	{
		loadStream?.Dispose();
	}
}

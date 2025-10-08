using System;
using System.Collections.Generic;
using System.Linq;
using StripClub.Gallery;
using StripClub.Model;
using UniRx;

namespace GreenT.HornyScapes.Gallery;

public class Gallery : IGallery, ICollectionSetter<Media>, ICollectionSetter<IMediaInfo>, IMediaInfoCollection
{
	private List<IMediaInfo> mediaInfos = new List<IMediaInfo>();

	private List<Media> mediaSet = new List<Media>();

	private Subject<Media> mediaAdded = new Subject<Media>();

	private Subject<Media> mediaReplaced = new Subject<Media>();

	public int MediaTotal { get; private set; }

	public IObservable<Media> OnMediaAdded => mediaAdded.AsObservable();

	public IObservable<Media> OnMediaReplaced => mediaReplaced.AsObservable();

	public string UniqueKey()
	{
		return "Gallery";
	}

	public IEnumerable<int> GetUniqueCharacterIDs()
	{
		return mediaSet.SelectMany((Media _media) => _media.Info.CharacterIDs).Distinct();
	}

	public IEnumerable<int> GetUniqueTagIDs()
	{
		List<int> tagIDs = new List<int>();
		mediaSet.ForEach(delegate(Media _media)
		{
			tagIDs.AddRange(_media.Info.tagIDs);
		});
		return tagIDs.Distinct();
	}

	public void Add(params IMediaInfo[] obj)
	{
		mediaInfos.AddRange(obj);
	}

	public void Add(params Media[] mediaArray)
	{
		foreach (Media media in mediaArray)
		{
			Media media2 = mediaSet.Find((Media infoMedia) => infoMedia.Info.ID == media.Info.ID);
			if (media2 != null)
			{
				media2.Thumbnail = media.Thumbnail;
				if (media2 is ImageMedia imageMedia)
				{
					if (media is ImageMedia imageMedia2)
					{
						imageMedia.Image = imageMedia2.Image;
					}
					else
					{
						imageMedia.Image = media.Thumbnail;
					}
				}
				mediaReplaced.OnNext(media2);
			}
			else
			{
				mediaSet.Add(media);
				mediaAdded.OnNext(media);
			}
		}
	}

	public bool TryGet(int mediaID, out Media media)
	{
		media = mediaSet.FirstOrDefault((Media x) => x.Info.ID == mediaID);
		return media != null;
	}

	public IEnumerable<Media> GetMedia(MediaFilter filter = null)
	{
		if (filter == null)
		{
			return mediaSet;
		}
		return mediaSet.Where((Media _media) => _media.Info.IsMatch(filter));
	}

	internal void SetTotal(int count)
	{
		MediaTotal = count;
	}

	public bool TryGet(int id, out IMediaInfo mediaInfo)
	{
		mediaInfo = mediaInfos.FirstOrDefault((IMediaInfo _info) => _info.ID == id);
		return mediaInfo != null;
	}

	public IEnumerable<IMediaInfo> Get(MediaFilter filter = null)
	{
		if (filter == null)
		{
			return mediaInfos;
		}
		return mediaInfos.Where((IMediaInfo _info) => _info.IsMatch(filter));
	}

	int IGallery.Count(MediaFilter filter)
	{
		if (filter == null)
		{
			return mediaSet.Count;
		}
		return mediaSet.Count((Media _media) => _media.Info.IsMatch(filter));
	}

	int IMediaInfoCollection.Count(MediaFilter filter)
	{
		if (filter == null)
		{
			return mediaInfos.Count;
		}
		return mediaInfos.Count((IMediaInfo _info) => _info.IsMatch(filter));
	}

	public void Purge()
	{
		mediaSet.Clear();
	}
}

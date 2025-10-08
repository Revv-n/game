using System;
using System.Collections.Generic;
using System.Linq;
using GreenT.Data;
using StripClub.Model;

namespace StripClub.Gallery;

public class Gallery : IGallery, ICollectionSetter<Media>
{
	private List<IMediaInfo> mediaInfos;

	private readonly ISaver saver;

	private List<Media> mediaSet = new List<Media>();

	public int MediaTotal { get; private set; }

	public event Action<IEnumerable<IMediaInfo>> OnUpdate;

	public string UniqueKey()
	{
		return "Gallery";
	}

	public Gallery(ISaver saver)
	{
		mediaInfos = new List<IMediaInfo>();
		this.saver = saver;
	}

	public IMediaInfo GetMediaInfo(int id)
	{
		return mediaInfos.SingleOrDefault((IMediaInfo media) => media.ID == id);
	}

	public IEnumerable<int> GetUniqueCharacterIDs()
	{
		return mediaInfos.SelectMany((IMediaInfo info) => info.CharacterIDs).Distinct();
	}

	public IEnumerable<int> GetUniqueTagIDs()
	{
		List<int> tagIDs = new List<int>();
		mediaInfos.ForEach(delegate(IMediaInfo info)
		{
			tagIDs.AddRange(info.tagIDs);
		});
		return tagIDs.Distinct();
	}

	public IEnumerable<IMediaInfo> GetMediaInfos(MediaFilter filter = null)
	{
		if (filter == null)
		{
			return mediaInfos;
		}
		return mediaInfos.Where((IMediaInfo info) => info.IsMatch(filter));
	}

	public void Add(IEnumerable<IMediaInfo> infoCollection)
	{
		foreach (IMediaInfo item in infoCollection)
		{
			mediaInfos.Add(item);
			if (item is ISavableState)
			{
				saver.Add(item as ISavableState);
			}
		}
		if (infoCollection.Any())
		{
			this.OnUpdate?.Invoke(infoCollection);
		}
	}

	public void Add(params Media[] obj)
	{
		mediaSet.AddRange(obj);
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
}

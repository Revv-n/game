using System;
using GreenT.AssetBundles;
using UnityEngine;
using Zenject;

namespace StripClub.Gallery.Data;

public class MediaFactory : IFactory<IMediaInfo, IAssetBundle, Media>, IFactory, IFactory<MediaMapper, IMediaInfo>
{
	private const string thumbnailFileName = "thumbnail";

	private const string mediaFileName = "media";

	public Media Create(IMediaInfo info, IAssetBundle asset)
	{
		asset.LoadAsset<Sprite>("thumbnail");
		UnityEngine.Object @object = asset.LoadAllAssets(info.Type)[0];
		if (@object is Sprite sprite)
		{
			return new ImageMedia(info, sprite, sprite);
		}
		throw new ArgumentOutOfRangeException("No behaviour for this type of media: " + @object.GetType());
	}

	public IMediaInfo Create(MediaMapper mapper)
	{
		if (mapper.type == "image")
		{
			Type typeFromHandle = typeof(Sprite);
			return new MediaInfo(mapper.id, typeFromHandle, mapper.char_id, mapper.tag_id);
		}
		Debug.LogError("There is no type for key word:mapper.Type");
		throw new ArgumentOutOfRangeException();
	}
}

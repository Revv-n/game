using System;
using StripClub.Model;
using UnityEngine;
using UnityEngine.Video;

namespace StripClub.Utility;

public static class Content
{
	public static Type ToType(MediaType contentType)
	{
		return contentType switch
		{
			MediaType.Photos => typeof(Sprite), 
			MediaType.Videos => typeof(VideoClip), 
			_ => throw new ArgumentOutOfRangeException("There is no behaviour for this type of content: " + contentType), 
		};
	}

	public static MediaType ToMediaType(Type type)
	{
		if ((object)type != null)
		{
			if (type.IsEquivalentTo(typeof(Sprite)))
			{
				return MediaType.Photos;
			}
			if (type.IsEquivalentTo(typeof(VideoClip)))
			{
				return MediaType.Videos;
			}
		}
		throw new ArgumentOutOfRangeException("There is no behaviour for this type : " + type.ToString());
	}
}

using System.Collections.Generic;
using GreenT.HornyScapes.MergeCore;
using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.Events;

public class BaseMergeIconProvider : IMergeIconProvider
{
	private readonly Dictionary<string, Sprite> _icons = new Dictionary<string, Sprite>();

	public void AddRange(IEnumerable<Sprite> sprites)
	{
		foreach (Sprite sprite in sprites)
		{
			Add(sprite);
		}
	}

	public void Add(Sprite sprite)
	{
		_icons[sprite.name] = sprite;
	}

	public Sprite GetSprite(GIKey key)
	{
		if (!_icons.TryGetValue(key.ToString(), out var value))
		{
			return null;
		}
		return value;
	}
}

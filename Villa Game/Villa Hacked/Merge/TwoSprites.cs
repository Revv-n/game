using System;
using UnityEngine;

namespace Merge;

[Serializable]
public struct TwoSprites
{
	public Sprite good;

	public Sprite bad;

	public Sprite GetSprite(bool value)
	{
		if (!value)
		{
			return bad;
		}
		return good;
	}
}

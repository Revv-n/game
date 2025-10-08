using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public class LegacyMergeIconProvider : IMergeIconProvider
{
	public Sprite GetSprite(GIKey key)
	{
		return IconProvider.GetGISprite(key);
	}
}

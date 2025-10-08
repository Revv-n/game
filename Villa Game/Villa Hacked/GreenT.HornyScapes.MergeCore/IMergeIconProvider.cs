using Merge;
using UnityEngine;

namespace GreenT.HornyScapes.MergeCore;

public interface IMergeIconProvider
{
	Sprite GetSprite(GIKey key);
}

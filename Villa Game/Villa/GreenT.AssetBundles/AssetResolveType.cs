using System;

namespace GreenT.AssetBundles;

[Flags]
public enum AssetResolveType
{
	Fake = 2,
	Silent = 4,
	Hard = 8
}

using System;
using UnityEngine;

namespace Merge;

[Serializable]
public struct TwoColors
{
	public Color good;

	public Color bad;

	public Color GetColor(bool value)
	{
		if (!value)
		{
			return bad;
		}
		return good;
	}
}

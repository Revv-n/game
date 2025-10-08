using System.Collections.Generic;
using UnityEngine;

namespace Merge;

public static class ListExtentions
{
	public static T GetRandom<T>(this List<T> list)
	{
		return list[Random.Range(0, list.Count)];
	}
}

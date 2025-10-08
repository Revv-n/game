using System;
using UnityEngine;

namespace Merge.Core.Tasks;

[Serializable]
public struct ObjectiveInfo
{
	[SerializeField]
	private GIKey item;

	[SerializeField]
	private int count;

	public GIKey Item => item;

	public int Count => count;

	public ObjectiveInfo(GIKey item, int count)
	{
		this.item = item;
		this.count = count;
	}
}

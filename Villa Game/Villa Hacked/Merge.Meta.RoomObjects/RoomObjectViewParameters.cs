using System;
using UnityEngine;

namespace Merge.Meta.RoomObjects;

[Serializable]
public class RoomObjectViewParameters : ViewParameters
{
	[SerializeField]
	private Sprite sprite;

	public Sprite Sprite => sprite;

	public RoomObjectViewParameters(Sprite sprite, int skinID, Vector2 offset, Vector2 scale)
		: base(skinID, offset, scale)
	{
		this.sprite = sprite;
	}
}

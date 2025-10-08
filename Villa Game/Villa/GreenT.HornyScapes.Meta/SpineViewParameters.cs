using System;
using Merge.Meta.RoomObjects;
using Spine.Unity;
using UnityEngine;

namespace GreenT.HornyScapes.Meta;

[Serializable]
public class SpineViewParameters : ViewParameters
{
	[field: SerializeField]
	public SpineAnimation Animation { get; }

	public SpineViewParameters(SpineAnimation animation, int skinID, Vector2 offset, Vector2 scale)
		: base(skinID, offset, scale)
	{
		Animation = animation;
	}
}

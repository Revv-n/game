using System;
using System.Collections.Generic;
using GreenT.HornyScapes.Animations;
using Merge.Meta.RoomObjects;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[Serializable]
public class RoomObjectViewInfo : ViewParameters<RoomObjectViewParameters>
{
	public RoomObjectViewInfo(Vector2 position, Vector2 localScale, int order, Vector2[] points, List<RoomObjectViewParameters> skinInfos, GreenT.HornyScapes.Animations.Animation beforeChangeAnimation, GreenT.HornyScapes.Animations.Animation afterChangeAnimation, Material material, Material blendMaterial, bool disableCollision)
		: base(position, localScale, order, points, skinInfos, beforeChangeAnimation, afterChangeAnimation, material, blendMaterial, disableCollision)
	{
	}
}

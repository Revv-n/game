using System;
using Spine.Unity;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[Serializable]
public class RoomObjectAnimationSettings : ICloneable
{
	public SkeletonAnimation SkeletonAnimation;

	public Vector3 Position;

	public Vector3 Scale;

	public RoomObjectAnimationSettings(SkeletonAnimation skeletonAnimation)
		: this(skeletonAnimation, Vector3.zero, Vector3.one)
	{
	}

	public RoomObjectAnimationSettings(SkeletonAnimation skeletonAnimation, Vector3 position, Vector3 scale)
	{
		SkeletonAnimation = skeletonAnimation;
		Position = position;
		Scale = scale;
	}

	public object Clone()
	{
		return new RoomObjectAnimationSettings(SkeletonAnimation, Position, Scale);
	}
}

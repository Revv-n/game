using System;
using UnityEngine;

namespace GreenT.HornyScapes.Animations;

[Serializable]
public class AnimationData
{
	public Vector3 Position;

	public Vector3 Rotation;

	public Vector3 Scale;

	public float Alpha = 1f;

	public AnimationData(Vector3 position, Vector3 rotation, Vector3 scale, float alpha)
	{
		Position = position;
		Rotation = rotation;
		Scale = scale;
		Alpha = alpha;
	}
}

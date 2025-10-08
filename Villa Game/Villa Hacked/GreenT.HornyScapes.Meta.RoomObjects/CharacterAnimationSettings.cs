using System;
using UnityEngine;

namespace GreenT.HornyScapes.Meta.RoomObjects;

[Serializable]
public class CharacterAnimationSettings : ICloneable
{
	[Serializable]
	public class ToolTipSetitngs
	{
		public Vector2 Position;

		public Vector2 Pivot;
	}

	public const string assetBundleRootPath = "employee/skins/{0}/animation";

	public Vector3 Position;

	public Vector3 Scale;

	public Vector2[] ColliderPoints;

	public ToolTipSetitngs ToolTipSettings;

	public CharacterAnimationSettings()
		: this(Vector3.zero, Vector3.one, new Vector2[0], new ToolTipSetitngs())
	{
	}

	public CharacterAnimationSettings(Vector3 position, Vector3 scale, Vector2[] colliderPoints, ToolTipSetitngs toolTipSettings)
	{
		Position = position;
		Scale = scale;
		ColliderPoints = colliderPoints;
		ToolTipSettings = toolTipSettings;
	}

	public object Clone()
	{
		return new CharacterAnimationSettings(Position, Scale, ColliderPoints, ToolTipSettings);
	}
}

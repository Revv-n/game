using System;
using UnityEngine;

namespace GreenT.HornyScapes.ToolTips;

public class ToolTipSettings : ScriptableObject, ICloneable
{
	public string KeyText;

	public Vector3 ToolTipPosition;

	public Vector2 PivotPosition;

	public Transform Parent;

	public ToolTipSettings Clone()
	{
		ToolTipSettings toolTipSettings = ScriptableObject.CreateInstance<ToolTipSettings>();
		toolTipSettings.KeyText = KeyText;
		toolTipSettings.ToolTipPosition = ToolTipPosition;
		toolTipSettings.PivotPosition = PivotPosition;
		toolTipSettings.Parent = Parent;
		return toolTipSettings;
	}

	object ICloneable.Clone()
	{
		return Clone();
	}
}
